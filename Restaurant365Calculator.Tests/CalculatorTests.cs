using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

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
