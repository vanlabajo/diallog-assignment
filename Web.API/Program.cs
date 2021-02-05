using CarRental;
using CarRental.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Linq;

namespace Web.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            CreateDbIfNotExists(host);

            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<DiallogDbContext>();
                context.Database.EnsureCreated();

                if (context.Cars.Any()) return;

                var cars = new Car[]
                {
                    new Car { Type = CarType.Sedan, Size = 4, GasConsumption = "1000 miles / liter", DailyRentalCost = 100, NumberOfUnits = 10 },
                    new Car { Type = CarType.SUV, Size = 6, GasConsumption = "1000 miles / liter", DailyRentalCost = 150, NumberOfUnits = 10 },
                    new Car { Type = CarType.Sedan, Size = 8, GasConsumption = "1000 miles / liter", DailyRentalCost = 200, NumberOfUnits = 10 }
                };

                context.Cars.AddRange(cars);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseSerilog((context, configuration) =>
                        {
                            configuration
                                .MinimumLevel.Debug()
                                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                .MinimumLevel.Override("System", LogEventLevel.Warning)
                                .Enrich.FromLogContext()
                                .WriteTo.File(@"Web.API.Log.txt")
                                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate);
                        });
                });
    }
}
