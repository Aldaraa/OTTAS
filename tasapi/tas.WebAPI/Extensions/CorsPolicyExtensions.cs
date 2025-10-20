using AspNetCoreRateLimit;
using System.Configuration;

namespace tas.WebAPI.Extensions
{
    public static class CorsPolicyExtensions
    {
        public static void ConfigureCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(o =>
            {

                o.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins(configuration.GetSection("AppSettings:Cors:AllowedOrigins").Get<string[]>())
                        .AllowCredentials();
                });
            });
        }


    }
}
