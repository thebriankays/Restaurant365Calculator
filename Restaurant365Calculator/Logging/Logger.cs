namespace Restaurant365Calculator.Logging
{
    public static class Logger
    {
        public static void Log(string message)
        {
            // Basic logging to console; this could be replaced with a more robust logging framework
            Console.WriteLine($"[LOG] {DateTime.Now}: {message}");
        }
    }
}