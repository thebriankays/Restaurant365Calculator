using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Restaurant365Calculator.Exceptions;

namespace Restaurant365Calculator.Tests
{
    public class CalculatorTests
    {
        private readonly Calculator _calculator;

        public CalculatorTests()
        {
            var mockLogger = new Mock<ILogger<Calculator>>();

            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockLogger.Object)
                .AddSingleton<Calculator>()
                .BuildServiceProvider();

            _calculator = serviceProvider.GetService<Calculator>() ?? throw new InvalidOperationException("Calculator service not configured properly.");
        }

        [Fact]
        public void Add_SingleNumber_ReturnsSameNumber()
        {
            Assert.Equal(20, _calculator.Add("20"));
        }

        [Fact]
        public void Add_TwoNumbers_ReturnsTheirSum()
        {
            Assert.Equal(5001, _calculator.Add("1,5000"));
        }

        [Fact]
        public void Add_NegativeNumber_ReturnsSum()
        {
            Assert.Equal(1, _calculator.Add("4,-3"));
        }

        [Fact]
        public void Add_EmptyInput_ReturnsZero()
        {
            Assert.Equal(0, _calculator.Add(""));
        }

        [Fact]
        public void Add_InvalidNumber_ReturnsSumWithZero()
        {
            Assert.Equal(5, _calculator.Add("5,tytyt"));
        }

        [Fact]
        public void Add_MoreThanTwoNumbers_ThrowsMaximumNumberException()
        {
            var exception = Assert.Throws<MaximumNumberException>(() => _calculator.Add("1,2,3"));
            Assert.Equal("A maximum of 2 numbers is allowed. Provided input: 1,2,3", exception.Message);
        }

        [Fact]
        public void Add_CommaOnlyInput_ReturnsZero()
        {
            Assert.Equal(0, _calculator.Add(","));
        }

        [Fact]
        public void Add_InputWithSpaces_ReturnsSumIgnoringSpaces()
        {
            Assert.Equal(5, _calculator.Add(" 2 , 3 "));
        }

        [Fact]
        public void Add_InputWithLeadingAndTrailingSpaces_ReturnsSum()
        {
            Assert.Equal(3, _calculator.Add(" 1,2 "));
        }

        [Fact]
        public void Add_NullInput_ReturnsZero()
        {
            Assert.Equal(0, _calculator.Add((string)null!));
        }

        [Fact]
        public void Add_LargeNumbers_HandlesCorrectly()
        {
            Assert.Equal(1000000, _calculator.Add("500000,500000"));
        }
    }
}
