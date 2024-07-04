using Microsoft.Extensions.Logging;
using Restaurant365Calculator.Exceptions;
using Restaurant365Calculator.Extensions;
using System.Text.RegularExpressions;

namespace Restaurant365Calculator
{
    /// <summary>
    /// Calculator class for performing arithmetic operations.
    /// </summary>
    public partial class Calculator(ILogger<Calculator> logger)
    {
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
                _logger.LogWarning("Warning: Input is null.");
                return 0;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(numbers))
                {
                    _logger.LogWarning("Warning: Input is empty.");
                    return 0;
                }

                var delimiters = new List<string> { ",", "\n" };
                var numberArray = ParseNumbers(numbers, delimiters);
                var negativeNumbers = numberArray.Where(n => n.ToInt() < 0).ToList();

                if (negativeNumbers.Count != 0)
                {
                    _logger.LogError("Error: Negative numbers found: {NegativeNumbers}", string.Join(", ", negativeNumbers));
                    throw new NegativeNumberException(negativeNumbers.Select(n => n.ToInt()));
                }

                var ignoredNumbers = numberArray.Where(n => n.ToInt() > 1000).ToList();
                if (ignoredNumbers.Count > 0)
                {
                    _logger.LogInformation("Info: Ignoring numbers greater than 1000: {IgnoredNumbers}", string.Join(", ", ignoredNumbers));
                }

                return numberArray.Where(n => n.ToInt() <= 1000).Sum(n => n.ToInt());
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
        /// <param name="delimiters">A list of delimiters to use for splitting the string.</param>
        /// <returns>An array of parsed numbers as strings.</returns>
        private string[] ParseNumbers(string numbers, List<string> delimiters)
        {
            var sanitizedInput = numbers.Trim();
            _logger.LogDebug("Parsing numbers from input: {Input}", sanitizedInput);

            if (sanitizedInput.StartsWith("//"))
            {
                var delimiterEndIndex = sanitizedInput.IndexOf('\n');
                var delimiterSection = sanitizedInput[2..delimiterEndIndex];

                var matches = MultipleDelimiterRegex().Matches(delimiterSection);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        delimiters.Add(match.Groups[1].Value);
                    }
                }
                else
                {
                    delimiters.Add(delimiterSection);
                }

                sanitizedInput = sanitizedInput[(delimiterEndIndex + 1)..];
            }

            var splitNumbers = sanitizedInput.Split(delimiters.ToArray(), StringSplitOptions.None)
                                              .Select(n => n.Trim())
                                              .ToArray();

            if (!IsValidInput(splitNumbers))
            {
                _logger.LogDebug("Warning: Invalid input format: {Input}. Invalid numbers will be converted to 0.", sanitizedInput);
            }

            return splitNumbers;
        }

        [GeneratedRegex(@"\[(.*?)\]")]
        private static partial Regex MultipleDelimiterRegex();

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
