using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Service
{
    public class FileAccessMiddleware : IMiddleware
    {
        private readonly ILogger<FileAccessMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public FileAccessMiddleware(ILogger<FileAccessMiddleware> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            
            var path = context.Request.Path.Value.ToLower();

            if (path.StartsWith("/resources/images/", StringComparison.OrdinalIgnoreCase))
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                var userPrincipal = context.User;
                var userRoles = userPrincipal.Claims.Where(x => x.Type == "TASSystemRole").Select(x => x.Value).ToArray();
                var hasRole = userRoles.Length > 0;
                var isGuest = userRoles.Contains("Guest");

                if (isGuest)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden");
                    return;
                }
                else
                {

                    await next(context);
                    //    // Check if user has one of the required roles
                    //if (!hasRole)
                    //{
                    //    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    //    await context.Response.WriteAsync("Forbidden");
                    //    return;
                    //}
                }
            }

            await next(context);
        }
    }


}
