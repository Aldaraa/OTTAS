using Microsoft.Extensions.FileProviders;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace tas.IntegrationAPI.Extensions
{
    public static class AppCommonExtension
    {
        public static void UseFileStaticAccess(this WebApplication app)
        {
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"logs")),
                RequestPath = new PathString("/log"),
                ServeUnknownFileTypes = true,
                DefaultContentType = "text/plain"
            });


        }


        public static void UseStaticConfiguration(this WebApplication app)
        {
            app.UseSwagger();


            app.UseSwaggerUI(c =>
            {
                c.DisplayRequestDuration();
                c.DocExpansion(DocExpansion.None);
                c.DocumentTitle = "TAS INTEGRATION API";
                c.EnableFilter();

            });

            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.Use(async (context, next) =>
            {
                var headers = context.Response.Headers;

                headers["X-Content-Type-Options"] = "nosniff";
                headers["X-Frame-Options"] = "DENY";
                headers["X-XSS-Protection"] = "1; mode=block";
                headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self'";

                await next();
            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseErrorHandler();


        }
    }
}
