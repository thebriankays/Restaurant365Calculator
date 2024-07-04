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
            logger.LogInformation("Result of Add(\"20\"): {Result}", calculator.Add("20"));                                                     // Output: 20
            logger.LogInformation("Result of Add(\"1,5000\"): {Result}", calculator.Add("1,5000"));                                             // Output: 5001
            logger.LogInformation("Result of Add(\"\"): {Result}", calculator.Add(""));                                                         // Output: 0
            logger.LogInformation("Result of Add(\"5,tytyt\"): {Result}", calculator.Add("5,tytyt"));                                           // Output: 5
            logger.LogInformation("Result of Add(\",\"): {Result}", calculator.Add(","));                                                       // Output: 0
            logger.LogInformation("Result of Add(\" 2 , 3 \"): {Result}", calculator.Add(" 2 , 3 "));                                           // Output: 5
            logger.LogInformation("Result of Add(\" 1,2 \"): {Result}", calculator.Add(" 1,2 "));                                               // Output: 3
            logger.LogInformation("Result of Add(\"1,2,3,4,5,6,7,8,9,10,11,12\"): {Result}", calculator.Add("1,2,3,4,5,6,7,8,9,10,11,12"));     // Output: 78
            logger.LogInformation("Result of Add(\"1\n2,3\"): {Result}", calculator.Add("1\n2,3"));                                             // Output: 6

            // Should throw NegativeNumberException
            try
            {
                logger.LogInformation("Result of Add(\"4,-3\"): {Result}", calculator.Add("4,-3"));
            }
            catch (NegativeNumberException ex)
            {
                logger.LogError(ex, "Exception caught: {Message}", ex.Message);
                Console.WriteLine(ex.Message);
            }

            try
            {
                calculator.Add("1,-2,3,-4");
            }
            catch (NegativeNumberException ex)
            {
                logger.LogError(ex, "Exception caught: {Message}", ex.Message);
                Console.WriteLine(ex.Message);
            }

            // Test case for ignoring numbers greater than 1000
            logger.LogInformation("Result of Add(\"2,1001,6\"): {Result}", calculator.Add("2,1001,6"));                                                   // Output: 8

            // Test case for custom delimiter
            logger.LogInformation("Result of Add(\"//#\n2#5\"): {Result}", calculator.Add("//#\n2#5"));                                                   // Output: 7
            logger.LogInformation("Result of Add(\"//,\n2,ff,100\"): {Result}", calculator.Add("//,\n2,ff,100"));                                         // Output: 102

            // Test case for custom delimiter of any length
            logger.LogInformation("Result of Add(\"//[***]\n11***22***33\"): {Result}", calculator.Add("//[***]\n11***22***33"));                         // Output: 66

            // Test case for multiple custom delimiters of any length
            logger.LogInformation("Result of Add(\"//[*][!!][r9r]\n11r9r22*hh*33!!44\"): {Result}", calculator.Add("//[*][!!][r9r]\n11r9r22*hh*33!!44")); // Output: 110
        }
    }
}
