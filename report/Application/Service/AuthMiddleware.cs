using Domain.Common;
using Domain.CustomModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class AuthMiddleware : IMiddleware
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        public AuthMiddleware(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
        }



        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            if (!context.User.Identity.IsAuthenticated)
            {
                await next(context);
            }
            else {
                if (context.User.Identity.Name != null)
                {
                    var outData = new AuthUser();

                    if (_memoryCache.TryGetValue($"REPORT_AUTH_{context.User.Identity.Name}", out outData))
                    {
                        await next(context);
                    }
                    else
                    {
                        var tasAPI = _configuration.GetSection("AppSettings:TASDomain").Value;
                        using (HttpClient client = new HttpClient())
                        {
                            var postData = new
                            {
                                username = context.User.Identity.Name
                            };
                            string json = JsonConvert.SerializeObject(postData);
                            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                            {
                                try
                                {
                                    HttpResponseMessage response = await client.PostAsync(tasAPI + "/api/auth/Auth/integration/userinfo", content);
                                    if (!response.IsSuccessStatusCode)
                                    {
                                        Console.WriteLine($"Error: {response.StatusCode}");
                                        return;
                                    }
                                    string responseContent = await response.Content.ReadAsStringAsync();
                                    AuthUser model = JsonConvert.DeserializeObject<AuthUser>(responseContent);
                                    if (model != null)
                                    {
                                        if (model.Role != "Guest")
                                        {

                                            _memoryCache.Set($"REPORT_AUTH_{context.User.Identity.Name}", model, TimeSpan.FromMinutes(GlobalConstants.TAS_MIDDLEWARE_AUTH_CACHE_MINUTE));
                                            await next(context);
                                        }
                                        else
                                        {
                                            context.Response.StatusCode = 401;
                                        }
                                    }
                                    else
                                    {
                                        context.Response.StatusCode = 401;
                                    }
                                }
                                catch (HttpRequestException e)
                                {
                                    context.Response.StatusCode = 401;
                                }
                            }
                        }
                    }

                }
                else
                {
                    context.Response.StatusCode = 401;
                }

            }



            //}
            //else
            //{
            //    await next(context);
            //  //  context.Response.StatusCode = 401;
            //    return;
            //}
        }
    }
}
