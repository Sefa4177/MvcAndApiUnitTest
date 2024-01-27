using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestWebExample.Web.Controllers;
using UnitTestWebExample.Web.Models;
using UnitTestWebExample.Web.Repository;
using Xunit;

namespace UnitTestWebExample.Test
{
    public class ProductsControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _productController;
        private List<Product> _products;

        public ProductsControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _productController = new ProductsController(_mockRepo.Object);
            _products = new List<Product>(){  
                               new Product{Id = 1, Name="Kalem", Color="Kırmızı", Stock=10, Price=100},
                               new Product{Id = 2, Name="Silgi", Color="Beyaz", Stock=20, Price=200},
                               new Product{Id = 3, Name="Defter", Color="Mavi", Stock=30, Price=300}
                               };  
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _productController.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(_products);

            var result = await _productController.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(3,productList.Count());
        }

        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _productController.Details(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Details_IdInValid_ReturnNotFound()
        {
            Product product = null;
            _mockRepo.Setup(repo => repo.GetById(0)).ReturnsAsync(product);

            var result = await _productController.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Details_ValidId_ReturnProduct(int productId)
        {
            Product product = _products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productController.Details(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _productController.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void CreatePOST_InvalidModelState_ReturnView()
        {
            _productController.ModelState.AddModelError("Name", "İsim alanı boş olamaz");

            var result = await _productController.Create(_products.First());

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _productController.Create(_products.First());

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_CreateMethodExecute()
        {
            Product newProduct = null;

            _mockRepo.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x => newProduct = x);

            var result = _productController.Create(_products.First());

            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once());

            Assert.Equal(_products.First().Id, newProduct.Id);
        }

        [Fact]
        public async void CreatePOST_InvalidModelState_NeverCreateMethodExecute()
        {
            _productController.ModelState.AddModelError("Name", "Name alanı boş olmamalı");

            var result = _productController.Create(_products.First());

            _mockRepo.Verify(repo => repo.Create(_products.First()), Times.Never());
        }

        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction() 
        {
            var result = await _productController.Edit(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(3)]
        public async void Edit_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productController.Edit(productId);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecutes_ReturnProduct(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productController.Edit(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            var result = _productController.Edit(2,_products.First(x => x.Id == productId));

            var redirect = Assert.IsType<NotFoundResult>(result);

        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_InValidModelState_ReturnView(int productId)
        {
            _productController.ModelState.AddModelError("Name", "");

            var result = _productController.Edit(productId, _products.First(x => x.Id == productId));

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {
            var result = _productController.Edit(productId, _products.First(x => x.Id == productId));

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_UpdateMethodExecute(int productId)
        {
            var product  = _products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.Update(product));

            _productController.Edit(productId, product);

            _mockRepo.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once); 
        }

        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _productController.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _productController.Delete(productId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecutes_ReturnProduct(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productController.Delete(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_ReturnRedirectToIndexAction(int productId)
        {
            var result = await _productController.DeleteConfirmed(productId);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ValidModelState_DeleteMethodExecute(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.Delete(product));

            await _productController.DeleteConfirmed(productId);

            _mockRepo.Verify(repo => repo.Delete (It.IsAny<Product>()), Times.Once);
        }


    }
}




























