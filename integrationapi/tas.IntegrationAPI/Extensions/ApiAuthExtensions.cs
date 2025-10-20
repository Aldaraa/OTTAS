
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System.Runtime.InteropServices;
using System.Text;

namespace tas.IntegrationAPI.Extensions
{
    public static class ApiAuthExtensions
    {
        public static void ConfigureSecurityAuth(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate(optionswindows =>
            {

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    optionswindows.EnableLdap(configuration.GetSection("AppSettings:Domain").Value);
                }
            });
            services.AddAuthorization();
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
                    Title = "TAS Integration API",
                    Version = "v1",
                    Description = "This API provides integration capabilities for the OTTAS Application.",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Davkharbayar Myagmar",
                        Email = "mdavkharbayar@gmail.com"
                    }
                });



                // Configure Swagger with JWT support

            });
        }

    }

   

}
