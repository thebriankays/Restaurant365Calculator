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
        public void Add_MultipleNumbers_ReturnsTheirSum()
        {
            Assert.Equal(78, _calculator.Add("1,2,3,4,5,6,7,8,9,10,11,12"));
        }

        [Fact]
        public void Add_NumbersWithNewlineDelimiter_ReturnsSum()
        {
            Assert.Equal(6, _calculator.Add("1\n2,3"));
        }

        [Fact]
        public void Add_NegativeNumbers_ThrowsNegativeNumberException()
        {
            var exception = Assert.Throws<NegativeNumberException>(() => _calculator.Add("1,-2,3,-4"));
            Assert.Equal("Negative numbers are not allowed: -2, -4", exception.Message);
            Assert.Contains(-2, exception.NegativeNumbers);
            Assert.Contains(-4, exception.NegativeNumbers);
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

        [Fact]
        public void Add_InputWithSpecialCharacters_ReturnsZero()
        {
            Assert.Equal(0, _calculator.Add("!,@"));
        }

        [Fact]
        public void Add_InputWithOnlyWhitespace_ReturnsZero()
        {
            Assert.Equal(0, _calculator.Add("   "));
        }
    }
}
