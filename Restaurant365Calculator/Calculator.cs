using Microsoft.Extensions.Logging;
using Restaurant365Calculator.Exceptions;
using Restaurant365Calculator.Extensions;

namespace Restaurant365Calculator
{
    /// <summary>
    /// Calculator class for performing arithmetic operations.
    /// </summary>
    public partial class Calculator(ILogger<Calculator> logger)
    {
        private static readonly List<string> DefaultDelimiters = [",", "\n"];
        private readonly ILogger<Calculator> _logger = logger ?? throw new NullLoggerException(nameof(logger));
        private static readonly char[] Separator = ['[', ']'];

        /// <summary>
        /// Adds numbers provided in a string.
        /// </summary>
        /// <param name="numbers">A string containing numbers to add.</param>
        /// <returns>The sum of the numbers and the formula used.</returns>
        public (int sum, string formula) Add(string? numbers)
        {
            if (numbers == null)
            {
                _logger.LogWarning("Warning: Input is null.");
                return (0, "0");
            }

            try
            {
                if (string.IsNullOrWhiteSpace(numbers))
                {
                    _logger.LogWarning("Warning: Input is empty.");
                    return (0, "0");
                }

                var numberArray = ParseNumbers(numbers, DefaultDelimiters);
                var negativeNumbers = numberArray.Where(n => n.ToInt() < 0).ToList();

                if (negativeNumbers.Count != 0)
                {
                    _logger.LogError("Error: Negative numbers found: {NegativeNumbers}", string.Join(", ", negativeNumbers));
                    throw new NegativeNumberException(negativeNumbers.Select(n => n.ToInt()));
                }

                var validNumbers = numberArray.Select(n => n.ToInt() > 1000 ? 0 : n.ToInt()).ToList();
                var formula = string.Join("+", validNumbers);
                var sum = validNumbers.Sum();

                return (sum, formula);
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
        /// <param name="delimiters">A list of delimiters.</param>
        /// <returns>An array of parsed numbers as strings.</returns>
        private static string[] ParseNumbers(string numbers, List<string> delimiters)
        {
            if (numbers.StartsWith("//"))
            {
                var delimiterEnd = numbers.IndexOf('\n');
                if (delimiterEnd == -1)
                {
                    throw new FormatException("Delimiter section not properly terminated with a newline.");
                }

                var delimiterSection = numbers[2..delimiterEnd];
                delimiters.AddRange(ParseDelimiters(delimiterSection));
                numbers = numbers[(delimiterEnd + 1)..];
            }

            var splitNumbers = numbers.Split(delimiters.ToArray(), StringSplitOptions.None);
            return splitNumbers;
        }

        /// <summary>
        /// Parses custom delimiters from the delimiter section.
        /// </summary>
        /// <param name="delimiterSection">The delimiter section of the input string.</param>
        /// <returns>A list of delimiters.</returns>
        private static List<string> ParseDelimiters(string delimiterSection)
        {
            var delimiters = new List<string>();
            var singleDelimiters = delimiterSection.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            delimiters.AddRange(singleDelimiters);
            return delimiters;
        }
    }
}
