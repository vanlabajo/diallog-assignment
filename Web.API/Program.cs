using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Web.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            //CreateDbIfNotExists(host);

            host.Run();
        }

        //private static void CreateDbIfNotExists(IHost host)
        //{
        //    using var scope = host.Services.CreateScope();
        //    var services = scope.ServiceProvider;
        //    try
        //    {
        //        var context = services.GetRequiredService<DiallogDbContext>();
        //        context.Database.EnsureCreated();

        //        // Look for any vegetables.
        //        if (context.Vegetables.Any())
        //        {
        //            return;   // DB has been seeded
        //        }

        //        var vegetables = new Vegetable[]
        //        {
        //        new Vegetable("Asparagus", 0.1M),
        //        new Vegetable("Beans", 0.15M),
        //        new Vegetable("Cabbages", 0.13M),
        //        new Vegetable("Chokos", 0.11M),
        //        new Vegetable("Cucumber", 0.18M),
        //        new Vegetable("Mushrooms", 0.2M)
        //        };

        //        context.Vegetables.AddRange(vegetables);
        //        context.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        var logger = services.GetRequiredService<ILogger<Program>>();
        //        logger.LogError(ex, "An error occurred creating the DB.");
        //    }
        //}

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
                                .WriteTo.File(@"API.Log.txt")
                                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate);
                        });
                });
    }
}
