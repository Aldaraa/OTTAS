using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AspNetCoreRateLimit;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace tas.Application.Service
{
    public class RateLimitAttribute : ActionFilterAttribute
    {
        public string Name { get; set; }
        public double Seconds { get; set; } = 0.5;
        private static MemoryCache Cache { get; } = new MemoryCache(new MemoryCacheOptions());


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var ipAddress = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
            var memoryCacheKey = $"{Name}-{ipAddress}";

            if (!Cache.TryGetValue(memoryCacheKey, out bool entry))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions() { 
                }
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(Seconds));
                Cache.Set(memoryCacheKey, true, cacheEntryOptions);
            }
            else
            {
                context.Result = new ContentResult
                {
                    Content = $"Requests are limited to 10, every {Seconds} seconds.",
                };
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            }

        }
    }
}
