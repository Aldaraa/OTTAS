using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using tas.Application.Common.Exceptions;
using tas.Application.Features.AuthenticationFeature.DepartmentRole;
using tas.Application.Service;
using tas.Application.Utils;
using tas.Domain.CustomModel;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public class HTTPUserRepository 
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly CacheService _cacheService;
        //  private readonly  int CacheMinute = 5;
        //    private readonly  IAuthenticationRepository _authenticationRepository;

        public HTTPUserRepository(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, CacheService cacheService)
        {

            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _cacheService = cacheService;
        }


        public void ClearRoleCache(string ADUsername)
        {

            try
            {
                //var cacheKeys = new List<string>
                //    {
                //        $"RoleData::{ADUsername}",
                //        $"RoleDepartmentData::{ADUsername}",
                //        $"Auth::{ADUsername}",
                //        $"LoginRequest::{ADUsername}",
                //        $"RoleData::{ADUsername.ToLowerInvariant()}",
                //        $"RoleDepartmentData::{ADUsername.ToLowerInvariant()}",
                //        $"Auth::{ADUsername.ToLowerInvariant()}",
                //        $"LoginRequest::{ADUsername.ToLowerInvariant()}",

                //        $"RoleData::{ADUsername.ToUpperInvariant()}",
                //        $"RoleDepartmentData::{ADUsername.ToUpperInvariant()}",
                //        $"Auth::{ADUsername.ToUpperInvariant()}",
                //        $"LoginRequest::{ADUsername.ToUpperInvariant()}",

                //        $"RoleData::{GenerateCamelCaseVariants(ADUsername)}",
                //        $"RoleDepartmentData::{GenerateCamelCaseVariants(ADUsername)}",
                //        $"Auth::{GenerateCamelCaseVariants(ADUsername)}",
                //        $"LoginRequest::{GenerateCamelCaseVariants(ADUsername)}"


                //    };

                _cacheService.RemoveBySuffix(ADUsername);

                //foreach (var cacheKey in cacheKeys)
                //{
                //    _cacheService.Remove(cacheKey);
                //}
            }
            catch (Exception ex)
            {
                // Handle exceptions if necessary
            }



        }


        private  string GenerateCamelCaseVariants(string input)
        {

            try
            {
                if (string.IsNullOrEmpty(input))
                    return input;

                var aa = input.Split(@"\")[0];

                var bb = input.Split(@"\")[1];

                var c = char.ToUpper(bb[0]) + bb.Substring(1).ToLower();


                return $"{aa}\\{c}";
            }
            catch (Exception)
            {
return input; ;
            }
            


        }

        public void ClearAllEmployeeCache()
        {
            _cacheService.RemoveByPrefix($"API::Employee");
        }


        public void ClearAllMasterCache<T>()
        {
            try
            {

                _cacheService.Remove($"API::{typeof(T).Name}_1");
                _cacheService.Remove($"API::{typeof(T).Name}_0");
                _cacheService.Remove($"API::{typeof(T).Name}");
                _cacheService.RemoveByPrefix($"API::{typeof(T).Name}");


                //var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
                //if (field == null)
                //    return;
                //var collection = field.GetValue(_memoryCache) as ICollection;
                //if (collection != null)
                //{
                //    foreach (var item in collection)
                //    {
                //        var methodInfo = item.GetType().GetProperty("Key");
                //        var val = methodInfo.GetValue(item);
                //        if (val.ToString().Contains($"API::{typeof(T).Name}"))
                //        {
                //            _memoryCache.Remove(val.ToString());
                //        }
                //    }
                //}
            }
            catch (Exception)
            {
            }
            
        }






        public TokenUserData? LogCurrentUser()
        {

           var returnData = new TokenUserData();
        //    var rmu = new RoleMiddlewareUser();
            if (_httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == "UserClaimId").Count() > 0)
            {

                returnData.Id = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == "UserClaimId").FirstOrDefault()?.Value);
                returnData.Role = _httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == "TASSystemRole").FirstOrDefault()?.Value;

                returnData.UserName = _httpContextAccessor.HttpContext.User.Claims.Where(x => x.Issuer == "AD AUTHORITY").FirstOrDefault()?.Value;

                string roleIdString = _httpContextAccessor.HttpContext.User.Claims
                .Where(x => x.Type == "TASSystemRoleID")
                .FirstOrDefault()?.Value;
                int roleId;
                if (int.TryParse(roleIdString, out roleId))
                {
                    returnData.RoleId = roleId;
                }
                else
                {
                    returnData.RoleId = null;
                }
                return returnData;
            }
            else {
                return null;
            }

        }








        private List<int> GetEmployeeIds()
        {

            DepartmentRoleResponse? rmuDepartment = null;

            var httpContext = new HttpContextAccessor().HttpContext;
            if (_memoryCache.TryGetValue($"RoleDepartmentData::{httpContext?.User.Identity?.Name}", out rmuDepartment))
            {

                if (rmuDepartment.EmployeeIds != null)
                {
                    return rmuDepartment.EmployeeIds;
                }
                else {

                  
                    var departmentIds = GetDepartmentsIds();

                    return new List<int>();

                }

            }
            else
            {
                return new List<int>();
            }
        }

        private List<int> GetDepartmentsIds()
        {

            DepartmentRoleResponse? rmuDepartment = null;

            var httpContext = new HttpContextAccessor().HttpContext;
            if (_memoryCache.TryGetValue($"RoleDepartmentData::{httpContext?.User.Identity?.Name}", out rmuDepartment))
            {
                return rmuDepartment.DepartmentsIds;
            }
            else
            {

                return new List<int>();
            }
        }

        public List<T> GetRoleEmpoyee<T>(List<T> mainData, string fieldName)
        {
            var role = LogCurrentUser()?.Role;
            var userId = LogCurrentUser()?.Id;


            if (role == "DepartmentAdmin" || role == "DepartmentManager")
            {
                var property = typeof(T).GetProperty(fieldName);

                if (property != null)
                {
                    var empIds = GetEmployeeIds();
            

                    return mainData.Where(data => empIds.Contains((int)property.GetValue(data))).ToList();
                }
                else
                {
                    return mainData;
                }
            }
            if (role == "Guest")
            {
                var property = typeof(T).GetProperty(fieldName);

                if (property != null)
                {
                    return mainData.Where(data => userId == (int)property.GetValue(data)).ToList();
                }
                else
                {
                    throw new ForBiddenException("You do not have access rights. Contact the administrator. Forbidden");
                }
            }



            return mainData;
        }

        public T GetRoleEmployeeObject<T>(T mainData, string fieldName)
        {
            if (mainData == null)
            {
                throw new ForBiddenException("You do not have access rights. Contact the administrator. Forbidden");
            }
            else {
                var role = LogCurrentUser()?.Role;
                var userId = LogCurrentUser()?.Id;
                if (role == "DepartmentAdmin" || role == "DepartmentManager") 
                {
                    var property = typeof(T).GetProperty(fieldName);

                    if (property != null)
                    {
                        var empIds = GetEmployeeIds();
                        var currentEmpId = property.GetValue(mainData, null);
                        if (empIds.IndexOf(Convert.ToInt32(currentEmpId)) != -1)
                        {
                            return mainData;
                        }
                        else
                        {
                            if (Convert.ToInt32(currentEmpId) == userId)
                            {
                                return mainData;
                            }
                            else
                            {
                                throw new ForBiddenException("You do not have access rights. Contact the administrator. Forbidden");
                            }
                        }

                    }
                    else
                    {
                        return mainData;
                    }
                }
                if (role == "Guest")
                {
                    var property = typeof(T).GetProperty(fieldName);

                    if (property != null)
                    {
                        var currentEmpId = property.GetValue(mainData, null);
                        if (userId.Value == Convert.ToInt32(currentEmpId))
                        {
                            return mainData;
                        }
                        else
                        {
                            throw new ForBiddenException("You do not have access rights. Contact the administrator. Forbidden");

                        }

                    }
                    else
                    {
                        return mainData;
                    }
                }

                return mainData;
            }
          
        }



        public record RoleMiddlewareUser
        {
            public int Id { get; set; }

            public string? Role { get; set; }
        }
    }
}
