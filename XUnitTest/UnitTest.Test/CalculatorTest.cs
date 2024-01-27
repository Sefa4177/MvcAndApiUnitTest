using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.APP;
using Xunit;

namespace UnitTest.Test
{
    public class CalculatorTest
    {
        public Calculator calculator { get; set; }
        public Mock<ICalculatorService> myMock { get; set; }
        public CalculatorTest()
        {
            myMock = new Mock<ICalculatorService>();
            this.calculator = new Calculator(myMock.Object); 
        }

        [Fact] // Normal methodun test methodu olarak tanımlanması için Fact atribute u eklenmesi gereklidir. Not : Fact sadece parametre almayan test methodları için kullanılırs
        public void AddTest()
        {
            // arrange = değişken tanımlama ve sınıf initialize etme işlemlerinin evresi
            int a = 5;
            int b = 6;

            // act = test edilcek sınıfın test edilecek methodunu çağırıp parametre girilme evresi
            var total = calculator.Add(a, b);

            // assert = act evresinde elde edilen sonucun doğru olup olmadıgının testinin yapıldgı evre
            Assert.Equal<int>(11, total);
        }

        [Theory]
        [InlineData(5,4,9)]
        [InlineData(6,8,14)]
        public void AddTest2(int a, int b, int expectedTotal) 
        { 
            myMock.Setup(x => x.Add(a, b)).Returns(expectedTotal);

            var actualTotal  = calculator.Add(a, b);

            Assert.Equal(expectedTotal, actualTotal);
        }
    }
}
