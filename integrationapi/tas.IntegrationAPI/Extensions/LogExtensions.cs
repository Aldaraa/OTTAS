using Serilog;
using Serilog.Events;

namespace tas.IntegrationAPI.Extensions
{
    public static class  LogExtensions
    {
        public static void ConfigureLog(this WebApplicationBuilder builder)
        {
            var logsPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            if (!Directory.Exists(logsPath))
            {
                Directory.CreateDirectory(logsPath);
            }

            // Configure Serilog
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information() // 👈 энд log level заана
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(Log.Logger);

        }

    }
}
