namespace Restaurant365Calculator.Exceptions
{
    public class NullLoggerException(string paramName) : ArgumentNullException(paramName, "Logger cannot be null")
    {
    }
}