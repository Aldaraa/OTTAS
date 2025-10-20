
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using tas.Application.Utils;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace tas.WebAPI.Extensions
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
                //if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                //{
                //    optionswindows.EnableLdap(configuration.GetSection("AppSettings:Domain").Value);
                //}
            });

                services.AddAntiforgery(options =>
                {
                    options.HeaderName = "X-CSRF-TOKEN";
                });
           


            services.AddHttpClient("client")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            });
           

            services.AddSwaggerGen(option =>
            {
                option.OperationFilter<FileUploadFilter>();

                option.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "TAS SYSTEM API",
                    Version = "v1",
                    Description = "The API for the OTTAS Application",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Davkharbayar Myagmar",
                        Email = "mdavkharbayar@gmail.com",
                        Url = new Uri("https://www.google.net"),
                    }
                });


                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //option.IncludeXmlComments(xmlPath);
            });

        }
    }
}
