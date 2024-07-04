using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Restaurant365Calculator.Exceptions;

namespace Restaurant365Calculator
{
    internal class Program
    {
        private static bool _keepRunning = true;

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

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true; 
                _keepRunning = false;
            };

            try
            {
                logger.LogInformation("Starting application");

                var calculator = serviceProvider.GetRequiredService<Calculator>();

                while (_keepRunning)
                {
                    Console.WriteLine("Enter numbers to add (or type 'exit' to quit):");
                    var input = Console.ReadLine();

                    // Delay to allow Ctrl+C event to be processed
                    Thread.Sleep(100);

                    if (!_keepRunning)
                    {
                        break;
                    }

                    if (input?.ToLower() == "exit")
                    {
                        break;
                    }

                    try
                    {
                        input = input?.Replace("\\n", "\n"); 

                        var (sum, formula) = calculator.Add(input);
                        Console.WriteLine($"{formula} = {sum}");
                        logger.LogInformation("Result of Add(\"{Input}\"): {Result}", input, sum);
                    }
                    catch (NegativeNumberException ex)
                    {
                        logger.LogError(ex, "Exception caught: {Message}", ex.Message);
                        Console.WriteLine(ex.Message);
                    }
                    catch (FormatException ex)
                    {
                        logger.LogError(ex, "Exception caught: {Message}", ex.Message);
                        Console.WriteLine("Invalid input format. Please try again.");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Exception caught: {Message}", ex.Message);
                        Console.WriteLine("An error occurred. Please try again.");
                    }
                }

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
    }
}
