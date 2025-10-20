using AspNetCoreRateLimit;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace tas.WebAPI.Extensions
{
    public static class RateLimitterMiddleware
    {

        public static void ConfigureRateLimitter(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            services.AddInMemoryRateLimiting();

            services.Configure<ClientRateLimitOptions>(options =>
            {
                options.EnableEndpointRateLimiting = true;
                options.StackBlockedRequests = false;
                options.HttpStatusCode = 429;
                options.ClientIdHeader = "Client-Id";
                options.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Endpoint = "*",
                        Period = "10s",
                        Limit = 2
                    }

                };
            });

        }
    }
}
