namespace Restaurant365Calculator.Extensions
{
    public static class StringExtensions
    {
        public static int ToInt(this string str)
        {
            return int.TryParse(str, out var result) ? result : 0;
        }
    }
}
