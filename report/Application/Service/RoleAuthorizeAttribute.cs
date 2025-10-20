using Domain.Common;
using Domain.CustomModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
    public class RoleAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        public RoleAuthorizeAttribute(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {

            if (context.HttpContext.User.Identity.Name != null)
            {
                var outData = new AuthUser();

                if (_memoryCache.TryGetValue($"REPORT_AUTH_{context.HttpContext.User.Identity.Name}", out outData))
                {
                    return ;
                }
                else
                {
                    var tasAPI = _configuration.GetSection("AppSettings:TASDomain").Value;
                    using (HttpClient client = new HttpClient())
                    {
                        var postData = new
                        {
                            username = context.HttpContext.User.Identity.Name
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

                                        _memoryCache.Set($"REPORT_AUTH_{context.HttpContext.User.Identity.Name}", model, TimeSpan.FromMinutes(GlobalConstants.TAS_MIDDLEWARE_AUTH_CACHE_MINUTE));
                                        return;
                                    }
                                    else
                                    {
                                        context.Result = new ForbidResult();
                                    }
                                }
                                else
                                {
                                    context.Result = new ForbidResult();
                                }
                            }
                            catch (HttpRequestException e)
                            {
                                context.Result = new ForbidResult();
                            }
                        }
                    }
                }

            }
            else
            {
                context.Result = new ForbidResult();
            }
        }




    }




}
