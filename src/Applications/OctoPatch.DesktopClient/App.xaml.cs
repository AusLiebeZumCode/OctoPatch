using Microsoft.Extensions.Logging;
using OctoPatch.Logging;
using Serilog;
using System.Windows;

namespace OctoPatch.DesktopClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .WriteTo.Console()
              .WriteTo.RollingFile("./logs/")
              .WriteTo.Seq("http://localhost:5341")
              .MinimumLevel.Debug()
              .CreateLogger();

            var loggerFactory = new LoggerFactory()
                .AddSerilog();

            LogManager.SetLoggerFactory(loggerFactory);
        }
    }
}
