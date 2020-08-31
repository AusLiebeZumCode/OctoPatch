using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace OctoConsole
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .WriteTo.Console()
              .WriteTo.File("./logs/.log", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
              .WriteTo.Seq("http://localhost:5341")
              .MinimumLevel.Debug()
              .CreateLogger();

            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(b =>
                {
                    b.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddSerilog();
                })
                .Build()
                .Run();
        }
    }
}
