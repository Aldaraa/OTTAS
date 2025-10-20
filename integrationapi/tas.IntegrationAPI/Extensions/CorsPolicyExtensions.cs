namespace tas.IntegrationAPI.Extensions
{
    public static class CorsPolicyExtensions
    {
        public static void ConfigureCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("AppSettings:Cors:AllowedOrigins").Get<string[]>();
            Console.WriteLine("Allowed Origins: " + string.Join(", ", allowedOrigins ?? new string[] { "None" }));

            services.AddCors(o =>
            {
                o.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .WithOrigins(allowedOrigins ?? new string[] { })
                          .AllowCredentials();
                });
            });
        }


    }
}
