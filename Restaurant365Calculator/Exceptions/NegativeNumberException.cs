namespace Restaurant365Calculator.Exceptions
{
    public class NegativeNumberException(IEnumerable<int> negativeNumbers) : Exception($"Negative numbers are not allowed: {string.Join(", ", negativeNumbers)}")
    {
        public IEnumerable<int> NegativeNumbers { get; } = negativeNumbers;
    }
}