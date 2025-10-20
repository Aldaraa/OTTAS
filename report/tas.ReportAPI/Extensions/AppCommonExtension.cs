using Microsoft.Extensions.FileProviders;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace tas.ReportAPI.Extensions
{
    public static class AppCommonExtension
    {
        public static void UseFileStaticAccess(this WebApplication app)
        {

            var assetDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
            if (!Directory.Exists(assetDirectory))
            {
                Directory.CreateDirectory(assetDirectory);
            }
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Assets")),
                RequestPath = new PathString("/Assets")
            });


            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(logDirectory),
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
                c.DocumentTitle = "TAS REPORT API";
                c.EnableFilter();

            });

            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseErrorHandler();
        }

    }
}
