using Microsoft.Extensions.Logging;
using Restaurant365Calculator.Exceptions;
using Restaurant365Calculator.Extensions;

namespace Restaurant365Calculator
{
    /// <summary>
    /// Calculator class for performing arithmetic operations.
    /// </summary>
    public class Calculator(ILogger<Calculator> logger)
    {
        private const char Delimiter = ',';
        private readonly ILogger<Calculator> _logger = logger ?? throw new NullLoggerException(nameof(logger));

        /// <summary>
        /// Adds numbers provided in a string.
        /// </summary>
        /// <param name="numbers">A string containing numbers to add.</param>
        /// <returns>The sum of the numbers.</returns>
        public int Add(string? numbers)  
        {
            if (numbers == null)
            {
                _logger.LogWarning("Input is null.");
                return 0;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(numbers))
                {
                    _logger.LogWarning("Warning: Input is empty.");
                    return 0;
                }

                var numberArray = ParseNumbers(numbers);

                if (numberArray.Length > 2)
                {
                    _logger.LogError("Error: A maximum of 2 numbers allowed. Provided input: {Numbers}", numbers);
                    throw new MaximumNumberException($"A maximum of 2 numbers allowed. Provided input: {numbers}");
                }

                return numberArray.Sum(n => n.ToInt());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: An error occurred while adding numbers.");
                throw;
            }
        }

        /// <summary>
        /// Parses numbers from a string.
        /// </summary>
        /// <param name="numbers">A string containing numbers to parse.</param>
        /// <returns>An array of parsed numbers as strings.</returns>
        private string[] ParseNumbers(string numbers)
        {
            var sanitizedInput = numbers.Trim();
            _logger.LogDebug("Parsing numbers from input: {Input}", sanitizedInput);

            var splitNumbers = sanitizedInput.Split(Delimiter).Select(n => n.Trim()).ToArray();

            if (!IsValidInput(splitNumbers))
            {
                _logger.LogDebug("Warning: Invalid input format: {Input}. Invalid numbers will be converted to 0.", sanitizedInput);
            }

            return splitNumbers;
        }

        /// <summary>
        /// Validates the input strings to check if they can be parsed as integers.
        /// </summary>
        /// <param name="splitNumbers">An array of strings to validate.</param>
        /// <returns>True if all strings are valid numbers or whitespace, otherwise false.</returns>
        private static bool IsValidInput(string[] splitNumbers)
        {
            return splitNumbers.All(n => int.TryParse(n, out _) || string.IsNullOrWhiteSpace(n));
        }
    }
}
