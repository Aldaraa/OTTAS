using AutoMapper.Configuration.Conventions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using tas.Application.Features.AuthenticationFeature.DepartmentRole;
using tas.Application.Features.AuthenticationFeature.LoginUser;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Service
{
    public class RoleMiddleware : IMiddleware
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _memoryCache;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly CacheService _cacheService;
        private readonly  int CacheMinute = 15;
        //private readonly RequestDelegate _next;

        public RoleMiddleware(IMediator mediator, IMemoryCache memoryCache, IAuthenticationRepository authenticationRepository/*, RequestDelegate next*/, CacheService cacheService)
        {
            _mediator = mediator;
            _memoryCache = memoryCache;
            _authenticationRepository = authenticationRepository;
            _cacheService = cacheService;
            // _next = next;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate _next)
        {

            var requestPath = context.Request.Path.Value.ToLower();
            if (!IsValidPath(requestPath))
            {
                await _next(context);
                return;
            }

            if (context.User.Identity.IsAuthenticated)
            {
                if (context.User.Identity.Name != null)
                {

                    LoginUserResponse? data = null;
                    RoleMiddlewareUser? rmu = null;
                    DepartmentRoleResponse? rmuDepartment = null;
                    if (_cacheService.TryGetValue($"RoleData::{context.User.Identity.Name}", out rmu))
                    {
                        //_memoryCache.Set($"RoleData::{context.User.Identity.Name}", rmu, TimeSpan.FromMinutes(CacheMinute));
                        var UserClaimId = new Claim("UserClaimId", rmu.Id.ToString());
                        var UserClaimRole = new Claim("TASSystemRole", rmu.Role.ToString());
                        var UserClaimRoleID = new Claim("TASSystemRoleID", rmu.RoleId != null ? rmu.RoleId.ToString() : string.Empty);
                        var UserClaimRoleReadOnlyAccess = new Claim("TASSystemReadOnlyAccess", "0");

                        var identity = (ClaimsIdentity)context.User.Identity;

                        var roleClaim = identity.FindFirst(ClaimTypes.Role);
                        if (roleClaim != null)
                        {
                            identity.RemoveClaim(roleClaim);
                        }

                        if (rmu.Role == "DepartmentAdmin" || rmu.Role == "DepartmentManager")
                        {
                            if (!_cacheService.TryGetValue($"RoleDepartmentData::{context.User.Identity.Name}", out rmuDepartment))
                            {
                                var rdata = await _authenticationRepository.GetDepartmentRoleData(new DepartmentRoleRequest(rmu.Id));
                                //   _memoryCache.Set($"RoleDepartmentData::{context.User.Identity.Name}", rdata, TimeSpan.FromMinutes(CacheMinute));
                                _cacheService.Set($"RoleDepartmentData::{context.User.Identity.Name}", rdata, CacheMinute);

                            }

                        }

                        var httpContext = new HttpContextAccessor().HttpContext;
                        var userPrincipal = httpContext.User;


                        var updatedClaimsIdentity = (ClaimsIdentity)userPrincipal.Identity;
                        updatedClaimsIdentity?.AddClaim(UserClaimRole);
                        updatedClaimsIdentity?.AddClaim(UserClaimId);
                        updatedClaimsIdentity?.AddClaim(UserClaimRoleID);
                        updatedClaimsIdentity?.AddClaim(UserClaimRoleReadOnlyAccess);
                        await _next(context);
                    }
                    else
                    {
                        data = await _authenticationRepository.LoginUserMiddleware(new LoginUserRequest(context.User.Identity.Name));
                        if (data != null)
                        {
                            rmu = new RoleMiddlewareUser();
                            rmu.Role = data.Role;
                            rmu.Id = data.Id;
                            rmu.ReadonlyAccess = data.ReadonlyAccess;
                            rmu.RoleId = data.RoleId;
                            //  _memoryCache.Set($"RoleData::{context.User.Identity.Name}", rmu, TimeSpan.FromMinutes(CacheMinute));

                            _cacheService.Set($"RoleData::{context.User.Identity.Name}", rmu, CacheMinute);

                            if (!_cacheService.TryGetValue($"Auth::{context.User.Identity.Name}", out data))
                            {
                                //   _memoryCache.Set($"Auth::{context.User.Identity.Name}", data, TimeSpan.FromMinutes(CacheMinute));
                                _cacheService.Set($"Auth::{context.User.Identity.Name}", data, CacheMinute);

                            }
                            var claims = context.User.Claims.Where(x => x.Type == ClaimTypes.Role);

                            var UserClaimId = new Claim("UserClaimId", rmu.Id.ToString());
                            var UserClaimRole = new Claim("TASSystemRole", rmu.Role.ToString());
                            var UserClaimRoleID = new Claim("TASSystemRoleID", rmu.RoleId != null ? rmu.RoleId.ToString() : string.Empty);
                            var UserClaimRoleReadOnlyAccess = new Claim("TASSystemReadOnlyAccess", "0");

                            var identity = (ClaimsIdentity)context.User.Identity;

                            var roleClaim = identity.FindFirst(ClaimTypes.Role);
                            if (roleClaim != null)
                            {
                                identity.RemoveClaim(roleClaim);
                            }


                            if (rmu.Role == "DepartmentAdmin" || rmu.Role == "DepartmentManager")
                            {
                                var rdata = await _authenticationRepository.GetDepartmentRoleData(new DepartmentRoleRequest(rmu.Id));
                                //  _memoryCache.Set($"RoleDepartmentData::{context.User.Identity.Name}", rdata, TimeSpan.FromMinutes(CacheMinute));
                                _cacheService.Set($"RoleDepartmentData::{context.User.Identity.Name}", rdata, CacheMinute);

                            }

                            var httpContext = new HttpContextAccessor().HttpContext;
                            var userPrincipal = httpContext?.User;
                            var updatedClaimsIdentity = (ClaimsIdentity)userPrincipal.Identity;
                            updatedClaimsIdentity?.AddClaim(UserClaimRole);
                            updatedClaimsIdentity?.AddClaim(UserClaimId);
                            updatedClaimsIdentity?.AddClaim(UserClaimRoleID);
                            updatedClaimsIdentity?.AddClaim(UserClaimRoleReadOnlyAccess);
                            await _next(context);
                        }
                        else
                        {
                            context.Response.StatusCode = 401;
                            return;
                        }
                    }
                }
                else {
                    context.Response.StatusCode = 401;
                    return;
                }
            }

               
           

        }

        //public async Task InvokeAsync(HttpContext context, RequestDelegate _next)
        //{

        //    if (context.Request.Path.Value.ToLower().Contains("hub"))
        //    {
        //        await _next(context);
        //        return;
        //    }

        //    if (context.Request.Path.Value.ToLower().Contains("api") || context.Request.Path.Value.ToLower().Contains("tashub") || context.Request.Path.Value.ToLower().Contains("screenhub"))
        //    {
        //        if (context.Request.Path.Value.ToLower().Contains("api") || context.Request.Path.Value.ToLower().Contains("tashub") || context.Request.Path.Value.ToLower().Contains("hub") || context.Request.Path.Value.ToLower().Contains("screenhub"))
        //        {

        //            if (context.User.Identity.IsAuthenticated)
        //            {
        //                if (context.User.Identity.Name != null)
        //                {

        //                    LoginUserResponse? data = null;
        //                    RoleMiddlewareUser? rmu = null;
        //                    DepartmentRoleResponse? rmuDepartment = null;
        //                    if (_cacheService.TryGetValue($"RoleData::{context.User.Identity.Name}", out rmu))
        //                    {
        //                        //_memoryCache.Set($"RoleData::{context.User.Identity.Name}", rmu, TimeSpan.FromMinutes(CacheMinute));
        //                        var UserClaimId = new Claim("UserClaimId", rmu.Id.ToString());
        //                        var UserClaimRole = new Claim("TASSystemRole", rmu.Role.ToString());
        //                        var UserClaimRoleID = new Claim("TASSystemRoleID", rmu.RoleId != null ? rmu.RoleId.ToString() : string.Empty);
        //                        var UserClaimRoleReadOnlyAccess = new Claim("TASSystemReadOnlyAccess", "0");

        //                        var identity = (ClaimsIdentity)context.User.Identity;

        //                        var roleClaim = identity.FindFirst(ClaimTypes.Role);
        //                        if (roleClaim != null)
        //                        {
        //                            identity.RemoveClaim(roleClaim);
        //                        }

        //                        if (rmu.Role == "DepartmentAdmin" || rmu.Role == "DepartmentManager")
        //                        {
        //                            if (!_cacheService.TryGetValue($"RoleDepartmentData::{context.User.Identity.Name}", out rmuDepartment))
        //                            {
        //                                var rdata = await _authenticationRepository.GetDepartmentRoleData(new DepartmentRoleRequest(rmu.Id));
        //                                //   _memoryCache.Set($"RoleDepartmentData::{context.User.Identity.Name}", rdata, TimeSpan.FromMinutes(CacheMinute));
        //                                   _cacheService.Set($"RoleDepartmentData::{context.User.Identity.Name}", rdata, CacheMinute);

        //                            }

        //                        }

        //                        var httpContext = new HttpContextAccessor().HttpContext;
        //                        var userPrincipal = httpContext.User;


        //                        var updatedClaimsIdentity = (ClaimsIdentity)userPrincipal.Identity;
        //                        updatedClaimsIdentity?.AddClaim(UserClaimRole);
        //                        updatedClaimsIdentity?.AddClaim(UserClaimId);
        //                        updatedClaimsIdentity?.AddClaim(UserClaimRoleID);
        //                        updatedClaimsIdentity?.AddClaim(UserClaimRoleReadOnlyAccess);
        //                        await _next(context); 
        //                    }
        //                    else
        //                    {
        //                        data = await _authenticationRepository.LoginUserMiddleware(new LoginUserRequest(context.User.Identity.Name));
        //                        if (data != null)
        //                        {
        //                            rmu = new RoleMiddlewareUser();
        //                            rmu.Role = data.Role;
        //                            rmu.Id = data.Id;
        //                            rmu.ReadonlyAccess = data.ReadonlyAccess;
        //                            rmu.RoleId = data.RoleId;
        //                            //  _memoryCache.Set($"RoleData::{context.User.Identity.Name}", rmu, TimeSpan.FromMinutes(CacheMinute));

        //                              _cacheService.Set($"RoleData::{context.User.Identity.Name}", rmu, CacheMinute);

        //                            if (!_cacheService.TryGetValue($"Auth::{context.User.Identity.Name}", out data))
        //                            {
        //                             //   _memoryCache.Set($"Auth::{context.User.Identity.Name}", data, TimeSpan.FromMinutes(CacheMinute));
        //                                _cacheService.Set($"Auth::{context.User.Identity.Name}", data,CacheMinute);

        //                            }
        //                            var claims = context.User.Claims.Where(x => x.Type == ClaimTypes.Role);

        //                            var UserClaimId = new Claim("UserClaimId", rmu.Id.ToString());
        //                            var UserClaimRole = new Claim("TASSystemRole", rmu.Role.ToString());
        //                            var UserClaimRoleID = new Claim("TASSystemRoleID", rmu.RoleId != null ? rmu.RoleId.ToString() : string.Empty);
        //                            var UserClaimRoleReadOnlyAccess = new Claim("TASSystemReadOnlyAccess", "0");

        //                            var identity = (ClaimsIdentity)context.User.Identity;

        //                            var roleClaim = identity.FindFirst(ClaimTypes.Role);
        //                            if (roleClaim != null)
        //                            {
        //                                identity.RemoveClaim(roleClaim);
        //                            }


        //                            if (rmu.Role == "DepartmentAdmin" || rmu.Role == "DepartmentManager" )
        //                            {
        //                                var rdata = await _authenticationRepository.GetDepartmentRoleData(new DepartmentRoleRequest(rmu.Id));
        //                                //  _memoryCache.Set($"RoleDepartmentData::{context.User.Identity.Name}", rdata, TimeSpan.FromMinutes(CacheMinute));
        //                                _cacheService.Set($"RoleDepartmentData::{context.User.Identity.Name}", rdata, CacheMinute);

        //                            }

        //                            var httpContext = new HttpContextAccessor().HttpContext;
        //                            var userPrincipal = httpContext?.User;
        //                            var updatedClaimsIdentity = (ClaimsIdentity)userPrincipal.Identity;
        //                            updatedClaimsIdentity?.AddClaim(UserClaimRole);
        //                            updatedClaimsIdentity?.AddClaim(UserClaimId);
        //                            updatedClaimsIdentity?.AddClaim(UserClaimRoleID);
        //                            updatedClaimsIdentity?.AddClaim(UserClaimRoleReadOnlyAccess);
        //                            await _next(context);
        //                        }
        //                        else
        //                        {
        //                            context.Response.StatusCode = 401;
        //                            return;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //todo report integration
        //                if (context.Request.Path.Value.ToLower().Contains("/api/auth/auth/integration/"))
        //                {
        //                    await _next(context);
        //                }
        //                else {
        //                    context.Response.StatusCode = 401;
        //                    return;
        //                }



        //            }

        //        }
        //    }

        //}

        private bool IsValidPath(string path)
        {
            string[] validPaths = { "api", "screenhub"};
            return validPaths.Any(p => path.Contains(p, StringComparison.OrdinalIgnoreCase));
        }

        //Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        //{
        //    throw new NotImplementedException();
        //}

        public record RoleMiddlewareUser
        {
            public int Id { get; set; }

            public string? Role { get; set; }

            public int? RoleId { get; set; }

            public int? ReadonlyAccess { get; set; }
        }
    }
}
