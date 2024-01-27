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
    public class ProductsApiControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsApiController _productApiController;

        private List<Product> _products;

        public ProductsApiControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _productApiController = new ProductsApiController(_mockRepo.Object);

            _products = new List<Product>(){
                               new Product{Id = 1, Name="Kalem", Color="Kırmızı", Stock=10, Price=100},
                               new Product{Id = 2, Name="Silgi", Color="Beyaz", Stock=20, Price=200},
                               new Product{Id = 3, Name="Defter", Color="Mavi", Stock=30, Price=300}
                               };
        }

        [Fact]
        public async void GetProduct_ActionExecutes_ReturnOkResultWithProducts()
        {
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(_products);

            var result = await _productApiController.GetProduct();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            Assert.Equal<int>(_products.Count(), returnProducts.ToList().Count());
        }

        [Theory]
        [InlineData(0)]
        public async void GetProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productApiController.GetProduct(productId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void GetProduct_IdValid_ReturnOkResultWithProducts(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productApiController.GetProduct(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsType<Product>(okResult.Value);

            Assert.Equal(product.Id, returnProduct.Id);
            Assert.Equal(product.Name, returnProduct.Name);
        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_IdIsNotEqualProduct_ReturnBadRequestResult(int productId)
        {
            var product = _products.First(x => x.Id == 2);

            var result = _productApiController.PutProduct(productId, product);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_ActionExecutes_ReturnNoContent(int productId)
        { 
            var product = _products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.Update(product));

            var result = _productApiController.PutProduct(productId, product);

            _mockRepo.Verify(x => x.Update(product),Times.Once);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void PostProduct_ActionExecutes_ReturnCreatedAtAction()
        {
            var product = _products.First();

            _mockRepo.Setup(repo => repo.Create(product)).Returns(Task.CompletedTask);

            var result = await _productApiController.PostProduct(product);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);

            _mockRepo.Verify(x => x.Create(product), Times.Once);

            Assert.Equal("GetProduct", createdAtActionResult.ActionName);
        }

        [Theory]
        [InlineData(0)]
        public async void DeleteProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var resultNotFound = await _productApiController.DeleteProduct(productId);

            Assert.IsType<NotFoundResult>(resultNotFound.Result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteProduct_ActionExecute_ReturnNoContent(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            _mockRepo.Setup(repo => repo.Delete(product));

            var noContentResult = await _productApiController.DeleteProduct(productId);

            _mockRepo.Verify(x => x.Delete(product), Times.Once);
            
            Assert.IsType<NoContentResult>(noContentResult.Result);
        }
    }
}
