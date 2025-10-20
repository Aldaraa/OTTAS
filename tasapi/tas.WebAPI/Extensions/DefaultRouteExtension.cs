using AspNetCoreRateLimit;
using DocumentFormat.OpenXml.InkML;
using System.Net;
using System.Text;
using System.Text.Json;
using tas.Application.Service;

namespace tas.WebAPI.Extensions
{
    public static class DefaultRouteExtension
    {
        public static void ConfigureDefaultRoute(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/api/home/index");
                    return Task.CompletedTask;
                });
            });

        }
       
    }
}
