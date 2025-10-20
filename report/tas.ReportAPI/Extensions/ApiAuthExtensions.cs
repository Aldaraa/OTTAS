using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.OpenApi.Models;
using System.Runtime.InteropServices;

namespace tas.ReportAPI.Extensions
{
    public static class ApiAuthExtensions
    {
        public static void ConfigureSecurityAuth(this IServiceCollection services, IConfiguration configuration)
        {
            //   services.AddScoped<AuthMiddleware>();
            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate(optionswindows =>
            {

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    optionswindows.EnableLdap(configuration.GetSection("AppSettings:Domain").Value);
                }
            });

            services.AddHttpClient("client")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            });


            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "TAS REPORT API",
                    Version = "v1",
                    Description = "The API for the OT.TAS.REPORT Application",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Davkahrbayar Myagmar",
                        Email = "mdavkharabayar@gmail.com",
                        Url = new Uri("https://www.google.net"),
                    }
                });
            });
        }
    }
}
