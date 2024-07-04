using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Restaurant365Calculator.Exceptions;

namespace Restaurant365Calculator
{
    internal class Program
    {
        private static void Main(string[] _)
        {
            var host = CreateHostBuilder().Build();

            using var scope = host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var logger = serviceProvider.GetService<ILogger<Program>>();

            if (logger == null)
            {
                Console.WriteLine("Logger not configured properly.");
                return;
            }

            try
            {
                logger.LogInformation("Starting application");

                var calculator = serviceProvider.GetRequiredService<Calculator>();

                RunCalculatorTests(calculator, logger);

                logger.LogInformation("Ending application");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "An unhandled exception occurred.");
            }
        }

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging(configure => configure.AddConfiguration(context.Configuration.GetSection("Logging")).AddConsole());
                    services.AddScoped<Calculator>();
                });

        /// <summary>
        /// Runs test cases for the calculator.
        /// </summary>
        /// <param name="calculator">The calculator instance to test.</param>
        /// <param name="logger">The logger instance for logging results.</param>
        private static void RunCalculatorTests(Calculator calculator, ILogger logger)
        {
            Console.WriteLine(calculator.Add("20"));      // Output: 20
            Console.WriteLine(calculator.Add("1,5000"));  // Output: 5001
            Console.WriteLine(calculator.Add("4,-3"));    // Output: 1
            Console.WriteLine(calculator.Add(""));        // Output: 0
            Console.WriteLine(calculator.Add("5,tytyt")); // Output: 5
            Console.WriteLine(calculator.Add(","));       // Output: 0
            Console.WriteLine(calculator.Add(" 2 , 3 ")); // Output: 5
            Console.WriteLine(calculator.Add(" 1,2 "));   // Output: 3

            try
            {
                Console.WriteLine(calculator.Add("1,2,3")); // Should throw MaximumNumberException
            }
            catch (MaximumNumberException ex)
            {
                logger.LogError(ex, "Exception caught: {Message}", ex.Message);
                Console.WriteLine(ex.Message);
            }
        }
    }
}
