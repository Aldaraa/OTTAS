using Microsoft.Extensions.FileProviders;
using Serilog;
using Serilog.Events;

namespace tas.ReportAPI.Extensions
{


    public static class LogExtensions
    {
        public static void ConfigureLogManagement(this WebApplicationBuilder builder)
        {
            // Серилог logger үүсгэх
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error() // 👈 энд log level
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Host болон Logging тохиргоо
            builder.Host.UseSerilog();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(Log.Logger);

        }


    }

}
