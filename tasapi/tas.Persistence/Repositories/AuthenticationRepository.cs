using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.AuthenticationFeature.AgreementCheck;
using tas.Application.Features.AuthenticationFeature.ClearADCache;
using tas.Application.Features.AuthenticationFeature.DepartmentRole;
using tas.Application.Features.AuthenticationFeature.ImpersoniteUser;
using tas.Application.Features.AuthenticationFeature.LoginUser;
using tas.Application.Features.AuthenticationFeature.RemoveUserCache;
using tas.Application.Features.DepartmentFeature.GetAllDepartment;
using tas.Application.Features.SysRoleEmployeeDashboardFeature.GetAllSysRoleEmployeeDashboard;
using tas.Application.Features.SysRoleMenuFeature.GetAllSysRoleMenu;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class AuthenticationRepository : BaseRepository<Employee>, IAuthenticationRepository
    {
        HTTPUserRepository _hTTPUserRepository;
        public AuthenticationRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _hTTPUserRepository = hTTPUserRepository;
        }

        public async Task AgreementCheck(AgreementCheckRequest request, CancellationToken cancellationToken)
        {

            var userData = _hTTPUserRepository.LogCurrentUser()?.Id;

            if (userData.HasValue)
            {

                var currentUser = await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userData.Value);
                if (currentUser != null)
                {
                    var employeeRole =await Context.SysRoleEmployees.AsNoTracking().Where(x => x.EmployeeId == currentUser.Id).FirstOrDefaultAsync(cancellationToken);

                    if (employeeRole != null)
                    {
                        employeeRole.Agreement = 1;
                        Context.SysRoleEmployees.Update(employeeRole);
                        await  Context.SaveChangesAsync();

                        if (!string.IsNullOrWhiteSpace(currentUser.ADAccount))
                        {

                            _hTTPUserRepository.ClearRoleCache(currentUser.ADAccount);
                        }

                    }

                }
            }



            await Task.CompletedTask;
        }


        public async Task ClearADCache(ClearADCacheRequest request, CancellationToken cancellationToken)
        {
            _hTTPUserRepository.ClearRoleCache(request.AdAccount);
            await Task.CompletedTask;
        }


        public async Task RemoveUserCache(RemoveUserCacheRequest request, CancellationToken cancellationToken) 
        {
            try
            {
                var currenrEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();
                if (currenrEmployee != null) {
                    if (!string.IsNullOrWhiteSpace(currenrEmployee.ADAccount)) {
                        _hTTPUserRepository.ClearRoleCache(currenrEmployee.ADAccount);
                       _hTTPUserRepository.ClearRoleCache(Convert.ToString(request.EmployeeId));

                    }
                }

            }
            catch (Exception)
            {
            }
        }






        public async Task<ImpersoniteUserResponse> ImpersoniteUserData(ImpersoniteUserRequest request, CancellationToken cancellationToken)
        {
            var currentUser = await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();

            var employeeRole = await Context.SysRoleEmployees.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId).FirstOrDefaultAsync();
            int? RoleId = null;
            if (employeeRole != null)
            {
                var currentRole = await Context.SysRole.AsNoTracking().Where(x => x.Id == employeeRole.RoleId).FirstOrDefaultAsync();
                if (currentRole != null)
                {


                    var returnData = new ImpersoniteUserResponse
                    {
                        Id = request.EmployeeId,
                        Lastname = currentUser?.Lastname,
                        Firstname = currentUser.Firstname,
                        Email = currentUser.Email,
                        Mobile = currentUser.Mobile,
                        NRN = currentUser.NRN,
                        SAPID = currentUser.SAPID,
                        ADAccount = currentUser.ADAccount,
                        Role = currentRole.Name,
                        RoleId = RoleId,

                    };

                    return returnData;

                }
                else {
                    throw new ForBiddenException("User Not Found\r\n\r\nThe system was unable to locate the requested user information. Access to the resource has been denied due to this failure.");
                }

            }
            else {
                throw new ForBiddenException("User Not Found\r\n\r\nThe system was unable to locate the requested user information. Access to the resource has been denied due to this failure.");
            }

        }



        public async Task<LoginUserResponse?> LoginUser(LoginUserRequest request, CancellationToken cancellationToken)
        {



            var currentUser = await Context.Employee.AsNoTracking().Where(x => x.ADAccount == request.username).FirstOrDefaultAsync();
           if (currentUser != null)
            {

                var guestRole = await Context.SysRole.AsNoTracking().Where(x => x.RoleTag == "Guest").FirstOrDefaultAsync();

                if (guestRole != null)
                {
                    var employeeRole = await Context.SysRoleEmployees.Where(x => x.EmployeeId == currentUser.Id && x.RoleId != guestRole.Id).FirstOrDefaultAsync();
                    string? rolename = null;
                    int? RoleId = null;
                    if (employeeRole != null)
                    {
                        var currentRole = await Context.SysRole.AsNoTracking().Where(x => x.Id == employeeRole.RoleId).FirstOrDefaultAsync();
                        if (currentRole != null)
                        {

                            /*THIS IS SYSTEM ROLE*/
                            rolename = currentRole.RoleTag;
                            RoleId = currentRole.Id;
                            employeeRole.LastLoginDate = DateTime.Now;
                            Context.SysRoleEmployees.Update(employeeRole);
                            await Context.SaveChangesAsync();


                            var menus = await GetRoleMenu(employeeRole.RoleId.Value, currentUser.Id, cancellationToken);
                            var dashboards = await GetRoleDashboard(currentUser.Id, cancellationToken);

                            var returnData = new LoginUserResponse
                            {
                                Id = currentUser.Id,

                                Lastname = currentUser.Lastname,
                                Firstname = currentUser.Firstname,
                                Email = currentUser.Email,
                                Mobile = currentUser.Mobile,
                                NRN = currentUser.NRN,
                                SAPID = currentUser.SAPID,
                                ADAccount = currentUser.ADAccount,
                                Role = rolename,
                                Agreement = employeeRole?.Agreement,
                                RoleId = RoleId,
                                CreateRequest = currentUser.CreateRequest,
                                ReadonlyAccess = employeeRole?.ReadonlyAccess == null ? 0 : employeeRole.ReadonlyAccess,
                                Menu = menus.Count > 0 ? menus : new List<LoginUserMenu>(),
                                Dashboard = dashboards

                            };

                            return returnData;


                        }
                        else
                        {

                            var supervisorRoleData = await Context.DepartmentSupervisor.AsNoTracking().Where(x => x.EmployeeId == currentUser.Id).FirstOrDefaultAsync();
                            if (supervisorRoleData == null)
                            {
                                /*THIS GUEST ROLE*/
                                rolename = guestRole.RoleTag;
                                RoleId = guestRole.Id;

                                var menus = await GetRoleMenu(guestRole.Id, currentUser.Id, cancellationToken);
                                var dashboards = await GetRoleDashboard(currentUser.Id, cancellationToken);
                                var returnData = new LoginUserResponse
                                {
                                    Id = currentUser.Id,
                                    Lastname = currentUser.Lastname,
                                    Firstname = currentUser.Firstname,
                                    Email = currentUser.Email,
                                    Mobile = currentUser.Mobile,
                                    NRN = currentUser.NRN,
                                    SAPID = currentUser.SAPID,
                                    ADAccount = currentUser.ADAccount,
                                    Role = rolename,
                                    RoleId = RoleId,
                                    Agreement = 1,
                                    ReadonlyAccess = 1,
                                    CreateRequest = currentUser.CreateRequest,
                                    Menu = menus.Count > 0 ? menus : new List<LoginUserMenu>(),
                                    Dashboard = dashboards

                                };

                                return returnData;
                            }
                            else {
                                var returnData = new LoginUserResponse
                                {
                                    Id = currentUser.Id,
                                    Lastname = currentUser.Lastname,
                                    Firstname = currentUser.Firstname,
                                    Email = currentUser.Email,
                                    Mobile = currentUser.Mobile,
                                    NRN = currentUser.NRN,
                                    SAPID = currentUser.SAPID,
                                    ADAccount = currentUser.ADAccount,
                                    Role = "Supervisor",
                                    RoleId = currentUser.Id,
                                    Agreement = 1,
                                    ReadonlyAccess = 1,
                                    CreateRequest = currentUser.CreateRequest,
                                    Menu = new List<LoginUserMenu>(),
                                    Dashboard = new List<LoginUserDashboard>()

                                };

                                return returnData;
                            }


                        }
                    }
                    else {


                        /*THIS GUEST and SUPERVISOR ROLE*/

                        var supervisorRoleData = await Context.DepartmentSupervisor.AsNoTracking().Where(x => x.EmployeeId == currentUser.Id).FirstOrDefaultAsync();
                        if (supervisorRoleData == null)
                        {
                            /*THIS GUEST ROLE*/
                            rolename = guestRole.RoleTag;
                            RoleId = guestRole.Id;

                            var menus = await GetRoleMenu(guestRole.Id, currentUser.Id, cancellationToken);
                            var dashboards = await GetRoleDashboard(currentUser.Id, cancellationToken);


                            var returnData = new LoginUserResponse
                            {
                                Id = currentUser.Id,
                                Lastname = currentUser.Lastname,
                                Firstname = currentUser.Firstname,
                                Email = currentUser.Email,
                                Mobile = currentUser.Mobile,
                                NRN = currentUser.NRN,
                                SAPID = currentUser.SAPID,
                                ADAccount = currentUser.ADAccount,
                                Role = rolename,
                                RoleId = RoleId,
                                Agreement = 1,
                                CreateRequest = currentUser.CreateRequest,
                                ReadonlyAccess = 1,
                                Menu = menus.Count > 0 ? menus : new List<LoginUserMenu>(),
                                Dashboard = dashboards

                            };

                            return returnData;
                        }
                        else
                        {
                            var returnData = new LoginUserResponse
                            {
                                Id = currentUser.Id,
                                Lastname = currentUser.Lastname,
                                Firstname = currentUser.Firstname,
                                Email = currentUser.Email,
                                Mobile = currentUser.Mobile,
                                NRN = currentUser.NRN,
                                SAPID = currentUser.SAPID,
                                ADAccount = currentUser.ADAccount,
                                Role = "Supervisor",
                                RoleId = currentUser.Id,
                                Agreement = 1,
                                ReadonlyAccess = 1,
                                Menu = new List<LoginUserMenu>(),
                                Dashboard = new List<LoginUserDashboard>()

                            };

                            return returnData;
                        }


                    }
                 
                }
                else {
                    throw new BadRequestException("Please system add Guest role");
                }
              

            }
            else {

                throw new ForBiddenException("Access denied");
            }
        }



        public async Task<List<LoginUserMenu>> GetRoleMenu(int RoleId, int EmployeeId, CancellationToken cancellationToken)
        {

            var result = new List<LoginUserMenu>();


           var employeeMenuIds = await Context.SysRoleEmployeeMenu.AsNoTracking().Where(x => x.EmployeeId == EmployeeId).Select(x=> x.MenuId).ToListAsync(cancellationToken);

            if (employeeMenuIds.Count > 0)
            {
                var menus = await Context.SysMenu.AsNoTracking().Where(x => employeeMenuIds.Contains(x.Id)).ToListAsync();

                foreach (var item in menus)
                {
                    var menuWithParents = await GetMenuWithParents(item.Id, cancellationToken);

                    foreach (var item2 in menuWithParents)
                    {

                        if (!result.Any(m => m.Id == item2.Id))
                        {
                            result.Add(item2);
                        }
                    }

                }

                return result;
            }
            else {
                var RoleMenuIds = await Context.SysRoleMenu.AsNoTracking().Where(x => x.RoleId == RoleId).Select(x => x.MenuId).ToListAsync(cancellationToken);

                var menus = await Context.SysMenu.AsNoTracking().Where(x => RoleMenuIds.Contains(x.Id)).ToListAsync();

                foreach (var item in menus)
                {
                    var menuWithParents = await GetMenuWithParents(item.Id, cancellationToken);

                    foreach (var item2 in menuWithParents)
                    {

                        if (!result.Any(m => m.Id == item2.Id))
                        {
                            result.Add(item2);
                        }
                    }

                }

                return result;


            }



         
        }



        public async Task<List<LoginUserDashboard>> GetRoleDashboard(int EmployeeId, CancellationToken cancellationToken)
        {
            var userRoleData =await Context.SysRoleEmployees.AsNoTracking().Where(x => x.EmployeeId == EmployeeId).FirstOrDefaultAsync();
            if (userRoleData != null)
            {
                var result = new List<LoginUserDashboard>();
                var userRole = await Context.SysRole.AsNoTracking().Where(x => x.Id == userRoleData.RoleId).FirstOrDefaultAsync();
                if (userRole != null)
                {
                    var employeeDashboardIds = await Context.SysRoleEmployeeDashboard.AsNoTracking().Where(x => x.EmployeeId == EmployeeId).Select(x => x.DashboardId).ToListAsync(cancellationToken);

                    if (employeeDashboardIds.Count > 0)
                    {
                        var sysDashboards = await Context.SysDashboard.AsNoTracking().Where(x => employeeDashboardIds.Contains(x.Id)).Select(x => new LoginUserDashboard
                        {
                            Code = x.Code,
                            Name = x.Name,
                            Id = x.Id
                        }).ToListAsync();

                        return sysDashboards;
                    }
                    else
                    {
                        if (userRole.RoleTag == "SystemAdmin")
                        {

                            var sysDashboards = await Context.SysDashboard.AsNoTracking().Select(x => new LoginUserDashboard
                            {
                                Code = x.Code,
                                Name = x.Name,
                                Id = x.Id
                            }).ToListAsync();

                            return sysDashboards;
                        }
                        else if (userRole.RoleTag == "AccomAdmin")
                        {
                            var sysDashboards = await Context.SysDashboard.AsNoTracking().Where(x => x.Code == "DASHBOARD_103").Select(x => new LoginUserDashboard
                            {
                                Code = x.Code,
                                Name = x.Name,
                                Id = x.Id
                            }).ToListAsync();

                            return sysDashboards;
                        }
                        else if (userRole.RoleTag == "TravelAdmin")
                        {
                            var sysDashboards = await Context.SysDashboard.AsNoTracking().Where(x => x.Code == "DASHBOARD_104").Select(x => new LoginUserDashboard
                            {
                                Code = x.Code,
                                Name = x.Name,
                                Id = x.Id
                            }).ToListAsync();

                            return sysDashboards;
                        }
                        else if (userRole.RoleTag == "DataApproval")
                        {
                            var sysDashboards = await Context.SysDashboard.AsNoTracking().Where(x => x.Code == "DASHBOARD_102").Select(x => new LoginUserDashboard
                            {
                                Code = x.Code,
                                Name = x.Name,
                                Id = x.Id
                            }).ToListAsync();

                            return sysDashboards;
                        }
                        else
                        {

                            return new List<LoginUserDashboard>();
                        }
                    }
                }
                else {
                    return new List<LoginUserDashboard>();
                }
            }
            else {

                return new List<LoginUserDashboard>();
            }

       




        }

        private async Task<List<LoginUserMenu>> GetMenuWithParents(int menuId, CancellationToken cancellationToken)
        {
            var result = new List<LoginUserMenu>();

            var menu = await Context.SysMenu.AsNoTracking().FirstOrDefaultAsync(x => x.Id == menuId, cancellationToken);

            if (menu != null)
            {
                var loginUserMenu = new LoginUserMenu
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    Code = menu.Code,
                    Route = menu.Route
                };

                result.Add(loginUserMenu);

                // Recursively find and include parent menus
                if (menu.ParentId.HasValue)
                {
                    var parentMenus = await GetMenuWithParents(menu.ParentId.Value, cancellationToken);
                    result.AddRange(parentMenus);
                }
            }

            return result;
        }





        public async Task<LoginUserResponse?> LoginUserMiddleware(LoginUserRequest request)
        {
            var currentUser = await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.ADAccount == request.username);
            if (currentUser != null)
            {

                var employeeRole =await Context.SysRoleEmployees.FirstOrDefaultAsync(x => x.EmployeeId == currentUser.Id);
                string? rolename = null;
                int? RoleId = null;
                if (employeeRole != null)
                {
                    var currentRole =await Context.SysRole.AsNoTracking().FirstOrDefaultAsync(x => x.Id == employeeRole.RoleId);
                    if (currentRole != null)
                    {
                        rolename = currentRole.RoleTag;
                        RoleId = currentRole.Id;
                    }

                    employeeRole.LastLoginDate = DateTime.Now;
                    Context.SysRoleEmployees.Update(employeeRole);
                    await Context.SaveChangesAsync();

                    var supervisorData = await Context.DepartmentSupervisor.AsNoTracking().Where(x => x.EmployeeId == currentUser.Id).FirstOrDefaultAsync();


                    if (rolename == null)
                    {
                        if (supervisorData != null)
                        {

                            var returnData = new LoginUserResponse
                            {
                                Id = currentUser.Id,
                                Lastname = currentUser.Lastname,
                                Firstname = currentUser.Firstname,
                                Email = currentUser.Email,
                                Mobile = currentUser.Mobile,
                                NRN = currentUser.NRN,
                                SAPID = currentUser.SAPID,
                                ADAccount = currentUser.ADAccount,
                                Role = "Supervisor",
                                CreateRequest = currentUser.CreateRequest,

                                RoleId = currentUser.Id,
                                Agreement = employeeRole.Agreement,
                                ReadonlyAccess = employeeRole.ReadonlyAccess == null ? 0 : employeeRole.ReadonlyAccess,
                                Menu = new List<LoginUserMenu>(),
                                Dashboard = new List<LoginUserDashboard>()

                            };

                            return returnData;
                        }
                        else
                        {

                            var returnData = new LoginUserResponse
                            {
                                Id = currentUser.Id,
                                Lastname = currentUser.Lastname,
                                Firstname = currentUser.Firstname,
                                Email = currentUser.Email,
                                Mobile = currentUser.Mobile,
                                NRN = currentUser.NRN,
                                SAPID = currentUser.SAPID,
                                ADAccount = currentUser.ADAccount,
                                Role = "Guest",
                                CreateRequest = currentUser.CreateRequest,
                                RoleId = currentUser.Id,
                                Agreement = employeeRole.Agreement,
                                ReadonlyAccess = employeeRole.ReadonlyAccess == null ? 0 : employeeRole.ReadonlyAccess,
                                Menu = new List<LoginUserMenu>(),
                                Dashboard = new List<LoginUserDashboard>()

                            };

                            return returnData;
                        }

                    }
                    else
                    {

                        var returnData = new LoginUserResponse
                        {
                            Id = currentUser.Id,
                            Lastname = currentUser.Lastname,
                            Firstname = currentUser.Firstname,
                            Email = currentUser.Email,
                            Mobile = currentUser.Mobile,
                            NRN = currentUser.NRN,
                            SAPID = currentUser.SAPID,
                            ADAccount = currentUser.ADAccount,
                            Role = rolename,
                            RoleId = RoleId,
                            CreateRequest = currentUser.CreateRequest,
                            Agreement = employeeRole.Agreement,
                            ReadonlyAccess = employeeRole.ReadonlyAccess == null ? 0 : employeeRole.ReadonlyAccess,
                            Menu = new List<LoginUserMenu>(),
                            Dashboard = new List<LoginUserDashboard>()

                        };

                        return returnData;
                    }
                }
                else {


                    var supervisorData = await Context.DepartmentSupervisor.AsNoTracking().Where(x => x.EmployeeId == currentUser.Id).FirstOrDefaultAsync();

                    if (supervisorData == null)
                    {
                        var returnData = new LoginUserResponse
                        {
                            Id = currentUser.Id,
                            Lastname = currentUser.Lastname,
                            Firstname = currentUser.Firstname,
                            Email = currentUser.Email,
                            Mobile = currentUser.Mobile,
                            NRN = currentUser.NRN,
                            SAPID = currentUser.SAPID,
                            ADAccount = currentUser.ADAccount,
                            Role = "Guest",
                            RoleId = currentUser.Id,
                            CreateRequest = currentUser.CreateRequest,
                            Agreement = 1,
                            ReadonlyAccess = 1,
                            Menu = new List<LoginUserMenu>(),
                            Dashboard = new List<LoginUserDashboard>()

                        };

                        return returnData;
                    }
                    else {
                        var returnData = new LoginUserResponse
                        {
                            Id = currentUser.Id,
                            Lastname = currentUser.Lastname,
                            Firstname = currentUser.Firstname,
                            Email = currentUser.Email,
                            Mobile = currentUser.Mobile,
                            NRN = currentUser.NRN,
                            SAPID = currentUser.SAPID,
                            ADAccount = currentUser.ADAccount,
                            Role ="Supervisor",
                            RoleId = currentUser.Id,
                            CreateRequest= currentUser.CreateRequest,   
                            Agreement = 1,
                            ReadonlyAccess = 1,
                            Menu = new List<LoginUserMenu>(),
                            Dashboard = new List<LoginUserDashboard>()


                        };

                        return returnData;
                    }

                }



            }
            else
            {

                return null;
            }
        }



        public async Task<DepartmentRoleResponse> GetDepartmentRoleData(DepartmentRoleRequest request) 
        {
            var employeeRoleId =await Context.SysRoleEmployees
                .Where(x => x.EmployeeId == request.UserId).FirstOrDefaultAsync();
            if (employeeRoleId != null)
            {
                var MainDepartmentIds =await Context.DepartmentAdmin
                    .Where(x => x.EmployeeId == request.UserId)
                    .Select(x => new { x.DepartmentId }).ToListAsync();

                if (MainDepartmentIds.Count > 0)
                {
                    List<int> departmentIds = new List<int>();
                    foreach (var item in MainDepartmentIds)
                    {
                        departmentIds.Add(item.DepartmentId);

                        departmentIds.AddRange(await PopulateChildren(item.DepartmentId));
                    

                    }


                    var empIds =await Context.Employee.Where(x => departmentIds.Contains(x.DepartmentId.Value))
                        .Select(x => x.Id).ToListAsync();

                    return new DepartmentRoleResponse
                    {
                        DepartmentsIds = departmentIds,
                        EmployeeIds = empIds
                    };

                }
                else{
                    return new DepartmentRoleResponse();
                }
            }
            else {
                return new DepartmentRoleResponse();
            }
        }

        //private async Task<List<int>> PopulateChildren(int parentId)
        //{
        //    var returnData = new List<int>();
        //  var childDepartments = await  Context.Department.Where(x => x.ParentDepartmentId == parentId)
        //        .Select(x => new { x.Id }).ToListAsync();

        //    foreach (var item in childDepartments)
        //    {
        //        returnData.Add(item.Id);
        //        returnData.AddRange(PopulateChildren(item.Id).Result);
        //    }   

        //    return returnData;


        //}

        //private async Task<List<int>> PopulateChildren(int parentId)
        //{
        //    var returnData = new List<int>();
        //    var childDepartments = await Context.Department
        //        .Where(x => x.ParentDepartmentId == parentId)
        //        .Select(x => new { x.Id })
        //        .ToListAsync();

        //    foreach (var item in childDepartments)
        //    {
        //        returnData.Add(item.Id);
        //        var children = await PopulateChildren(item.Id);
        //        returnData.AddRange(children);
        //    }

        //    return returnData;
        //}

        private async Task<List<int>> PopulateChildren(int parentId)
        {
            var returnData = new List<int>();
            var childDepartments = await Context.Department
                .Where(x => x.ParentDepartmentId == parentId)
                .Select(x => new { x.Id })
                .ToListAsync();

            foreach (var item in childDepartments)
            {
                returnData.Add(item.Id);
                var children = await PopulateChildren(item.Id);
                returnData.AddRange(children);
            }

            return returnData;
        }


    }
}
