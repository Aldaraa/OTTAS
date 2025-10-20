using Application.Common.Exceptions;
using Domain.CustomModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class HTTPUserRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;

        public HTTPUserRepository(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {

            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
        }




        public AuthUser? LogCurrentUser()
        {

            var outData = new AuthUser();

            if (_memoryCache.TryGetValue($"REPORT_AUTH_{_httpContextAccessor.HttpContext.User.Identity.Name}", out outData))
            {
                return outData;
            }
            else { 
            return null;
            }

        }







        public record RoleMiddlewareUser
        {
            public int Id { get; set; }

            public string Name { get; set; }



            public string? Role { get; set; }
        }
    }
}
