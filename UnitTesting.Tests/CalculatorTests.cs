using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTesting;

namespace UnitTesting.Tests
{
    [TestClass]
    public class CalculatorTests
    {
        private Calculator _calculator = null!;

        [TestInitialize]
        public void Setup()
        {
            _calculator = new Calculator();
        }

        [TestMethod]
        public void Add_ShouldReturnCorrectResult()
        {
            int result = _calculator.Add(2, 3);
            Assert.AreEqual(5, result);
        }

        [DataTestMethod]
        [DataRow(10, 3, 7)]
        [DataRow(5, 5, 0)]
        [DataRow(0, 7, -7)]
        public void Subtract_ShouldReturnCorrectResult(int a, int b, int expected)
        {
            int result = _calculator.Subtract(a, b);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Multiply_ShouldReturnCorrectResult()
        {
            int result = _calculator.Multiply(3, 4);
            Assert.AreEqual(12, result);
        }

        [TestMethod]
        public void Square_ShouldReturnCorrectResult()
        {
            int result = _calculator.Square(5);
            Assert.AreEqual(25, result);
        }

        [TestMethod]
        public void Divide_ShouldReturnCorrectResult()
        {
            int result = _calculator.Divide(10, 2);
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void Divide_ShouldThrowException_WhenDividingByZero()
        {
            Assert.ThrowsException<DivideByZeroException>(() => _calculator.Divide(10, 0));
        }
    }
}
