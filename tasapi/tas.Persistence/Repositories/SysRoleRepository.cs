using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.AuthenticationFeature.LoginUser;
using tas.Application.Features.SysRoleEmployeeMenuFeature.GetAllSysRoleEmployeeMenu;
using tas.Application.Features.SysRoleFeature.AddEmployee;
using tas.Application.Features.SysRoleFeature.GetAllSysRole;
using tas.Application.Features.SysRoleFeature.GetEmployeeRoleInfo;
using tas.Application.Features.SysRoleFeature.GetSysRole;
using tas.Application.Features.SysRoleFeature.RemoveEmployeeRole;
using tas.Application.Features.SysRoleFeature.UpdateReadOnlyAccesss;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class SysRoleRepository : BaseRepository<SysRole>, ISysRoleRepository
    {
        private readonly IConfiguration _Configuration;
        private readonly CacheService _memoryCache;
        private readonly HTTPUserRepository _userRepository;
        private readonly SignalrHub _signalrHub;
        public SysRoleRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, CacheService memoryCache, SignalrHub signalrHub) : base(context, configuration, hTTPUserRepository)
        {
            _Configuration = configuration; 
            _userRepository = hTTPUserRepository;
            _memoryCache = memoryCache;
            _signalrHub = signalrHub;
        }

        public async Task<List<GetAllSysRoleResponse>> GetAllData(CancellationToken cancellationToken)
        {
           var roles = await Context.SysRole.ToListAsync(cancellationToken);
            var returnData  = new List<GetAllSysRoleResponse>();
            foreach (var item in roles)
            {
                var role = new GetAllSysRoleResponse
                {
                    Id = item.Id,
                    Description = item.Description,
                    DataPermission = item.DataPermission,
                    Name = item.Name,
                    EmployeeCount = item.RoleTag == "Guest" ? -1 : Context.SysRoleEmployees.Count(x => x.RoleId == item.Id)
                };
            returnData.Add(role);
                


            }

            return returnData;
        }

        public async Task RemoveEmployee(RemoveEmployeeRoleRequest request, CancellationToken cancellationToken)
        {
            var roleData =await Context.SysRoleEmployees.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (roleData != null)
            {

                var adminData = await Context.DepartmentAdmin.Where(x => x.EmployeeId == roleData.EmployeeId).ToListAsync();
                foreach (var item in adminData)
                {
                    Context.DepartmentAdmin.Remove(item);
                }


                var managerData = await Context.DepartmentManager.Where(x => x.EmployeeId == roleData.EmployeeId).ToListAsync();
                foreach (var item in managerData)
                {
                    Context.DepartmentManager.Remove(item);
                }

                var supervisorData = await Context.DepartmentSupervisor.Where(x => x.EmployeeId == roleData.EmployeeId).ToListAsync();
                foreach (var item in supervisorData)
                {
                    Context.DepartmentSupervisor.Remove(item);
                }
                Context.SysRoleEmployees.Remove(roleData);




                var requestLineManagers = await Context.RequestLineManagerEmployee
                    .Where(x => x.LineManagerId == roleData.EmployeeId)
                    .ToListAsync();
                foreach (var data in requestLineManagers)
                {
                    Context.RequestLineManagerEmployee.Remove(data);
                }


                var requestLineManagerEmployees = await Context.RequestLineManagerEmployee
                .Where(x => x.EmployeeId == roleData.EmployeeId)
                .ToListAsync();
                foreach (var data in requestLineManagerEmployees)
                {
                    Context.RequestLineManagerEmployee.Remove(data);
                }

                //var requestDelegatesTo = await Context.RequestDelegates
                //            .Where(x => x.ToEmployeeId == roleData.EmployeeId)
                //            .ToListAsync();
                //foreach (var data in requestDelegatesTo)
                //{
                //    Context.RequestDelegates.Remove(data);
                //}


                var EmployerAdminData = await Context.EmployerAdmin
                            .Where(x => x.EmployeeId == roleData.EmployeeId)
                            .ToListAsync();

                foreach (var data in EmployerAdminData)
                {
                    Context.EmployerAdmin.Remove(data);
                }


                var employeeMenu = await Context.SysRoleEmployeeMenu.Where(x => x.EmployeeId == roleData.EmployeeId).ToListAsync();

                if (employeeMenu.Count > 0)
                {
                    Context.SysRoleEmployeeMenu.RemoveRange(employeeMenu);
                }


                var employeeDashboard = await Context.SysRoleEmployeeDashboard.Where(x => x.EmployeeId == roleData.EmployeeId).ToListAsync();

                if (employeeMenu.Count > 0)
                {
                    Context.SysRoleEmployeeDashboard.RemoveRange(employeeDashboard);
                }

                var employeeReport = await Context.SysRoleEmployeeReportTemplate.Where(x => x.EmployeeId == roleData.EmployeeId).ToListAsync();

                if (employeeReport.Count > 0)
                {
                    Context.SysRoleEmployeeReportTemplate.RemoveRange(employeeReport);
                }


                var DepartmentManagers = await Context.DepartmentManager.Where(x => x.EmployeeId == roleData.EmployeeId).ToListAsync();

                if (DepartmentManagers.Count > 0)
                {
                    Context.DepartmentManager.RemoveRange(DepartmentManagers);
                }





                var SysRoleEmployeeReportDepartment = await Context.SysRoleEmployeeReportDepartment.Where(x => x.EmployeeId == roleData.EmployeeId).ToListAsync();

                if (SysRoleEmployeeReportDepartment.Count > 0)
                {
                    Context.SysRoleEmployeeReportDepartment.RemoveRange(SysRoleEmployeeReportDepartment);
                }

                var SysRoleEmployeeReportEmployer = await Context.SysRoleEmployeeReportEmployer.Where(x => x.EmployeeId == roleData.EmployeeId).ToListAsync();

                if (SysRoleEmployeeReportEmployer.Count > 0)
                {
                    Context.SysRoleEmployeeReportEmployer.RemoveRange(SysRoleEmployeeReportEmployer);
                }

                var delegationAccess = await Context.RequestDelegates
                    .Where(x => x.FromEmployeeId == roleData.EmployeeId || x.ToEmployeeId == roleData.EmployeeId).ToListAsync();

                if (delegationAccess.Count > 0) {
                    Context.RequestDelegates.RemoveRange(delegationAccess);
                }


               var groupData =await Context.RequestGroupEmployee.Where(x => x.EmployeeId == roleData.EmployeeId).ToListAsync();
                if (groupData.Count > 0) {
                    Context.RequestGroupEmployee.RemoveRange(groupData);
                }



                var currentEmployeeAdInfo = await Context.Employee.AsNoTracking().Where(x => x.Id == roleData.EmployeeId).Select(x=> new {x.ADAccount}).FirstOrDefaultAsync();

                if (!string.IsNullOrWhiteSpace(currentEmployeeAdInfo?.ADAccount))
                {
                    _userRepository.ClearRoleCache(currentEmployeeAdInfo.ADAccount);
                }

                await _signalrHub.RoleChange(Convert.ToString(roleData?.EmployeeId));

                await Task.CompletedTask;


            }
            else {
              await  Task.CompletedTask;
            }

        }


        public async Task UpdateReadOnlyAccesss(UpdateReadOnlyAccesssRequest request, CancellationToken cancellationToken)
        {
            var currentEmployeeRole =await Context.SysRoleEmployees.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (currentEmployeeRole != null)
            {
                currentEmployeeRole.ReadonlyAccess = request.ReadOnlyAccess;

                Context.SysRoleEmployees.Update(currentEmployeeRole);


                ActiveDirectoryService activeDirectoryService = new ActiveDirectoryService();

                var currentUser = await Context.Employee.AsNoTracking().Where(x => x.Id == currentEmployeeRole.EmployeeId).Select(x => new { x.ADAccount, x.Id }).FirstOrDefaultAsync();
                if (currentUser != null)
                {
                    var user = activeDirectoryService.GetUserFromAd(_Configuration.GetSection("AppSettings:Domain").Value, currentUser.ADAccount);
                    if (user != null)
                    {
                        _memoryCache.Remove($"RoleData::{user.UserName?.ToLower()}");
                    }


                }

            }
        }


        public async Task AddEmployee(AddEmployeeRequest request, CancellationToken cancellationToken)
        {

            var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();
            if (currentEmployee != null)
            {
                if (currentEmployee.Active == 1)
                {
                    var currentRole = await Context.SysRole.AsNoTracking().Where(x => x.Id == request.RoleId).FirstOrDefaultAsync();

                    if (currentRole != null)
                    {
                        await ValidateEmployeeRole(request);



                        SysRoleEmployees newData = new SysRoleEmployees
                        {
                            Active = 1,
                            DateCreated = DateTime.Now,
                            EmployeeId = request.EmployeeId,
                            RoleId = request.RoleId,
                            ReadonlyAccess = request.ReadOnlyAccess,
                            UserIdCreated = _userRepository.LogCurrentUser()?.Id
                        };

                        Context.SysRoleEmployees.Add(newData);

                        if (currentRole.RoleTag == "DataApproval")
                        {
                            var requestGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.GroupTag == "dataapproval").FirstOrDefaultAsync();
                            if (requestGroup != null)
                            {
                                var hasAlreadyGroup = await Context.RequestGroupEmployee.AsNoTracking().Where(x => x.RequestGroupId == requestGroup.Id && x.EmployeeId == request.EmployeeId).AnyAsync();
                                if (!hasAlreadyGroup)
                                {
                                    var newGroupRecord = new RequestGroupEmployee
                                    {
                                        Active = 1,
                                        DateCreated = DateTime.Now,
                                        DisplayName = currentEmployee.Firstname,
                                        EmployeeId = request.EmployeeId,
                                        PrimaryContact = 0,
                                        RequestGroupId = requestGroup.Id,
                                        UserIdCreated = _userRepository.LogCurrentUser()?.Id,
                                        OrderIndex = 0
                                        
                                    };

                                    Context.RequestGroupEmployee.Add(newGroupRecord);
                                }

                            }
                        }

                        var currentUser = await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId).Select(x => new { x.ADAccount, x.Id }).FirstOrDefaultAsync();
                        if (currentUser != null)
                        {
                            if (!string.IsNullOrWhiteSpace(currentUser?.ADAccount))
                            {
                                _userRepository.ClearRoleCache(currentUser.ADAccount);
                            }
                            await _signalrHub.RoleChange(Convert.ToString(request.EmployeeId));
                        }
                    }
                }
                else {
                    throw new BadRequestException("Action cannot be completed because the employee's status is inactive. Please contact the system administrator ");
                }
              
            }
            else {
                throw new BadRequestException("Action cannot be completed because the employee's status is inactive. Please contact the system administrator ");
            }
        

        }

        private async Task ValidateEmployeeRole(AddEmployeeRequest request)
        {
            var currentData = await Context.SysRoleEmployees.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeId == request.EmployeeId);
            if (currentData != null) {

                var currentRole = await Context.SysRole.AsNoTracking().Where(x=> x.Id == currentData.RoleId).Select(x=> x.Name).FirstOrDefaultAsync();

                throw new BadRequestException($"\"'{currentRole}' rights have already been assigned to this user. Creating a new role is not possible. Please delete your current role instead.\"");
            }
        }



        public async Task<GetSysRoleResponse> GetData(GetSysRoleRequest request, CancellationToken cancellationToken)
        {
            var currentRole =await Context.SysRole.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.RoleId);
            if (currentRole != null)
            {
                var returnData = new GetSysRoleResponse
                {
                    Id = currentRole.Id,
                    Description = currentRole.Description,
                    Name = currentRole.Name,
                    RoleTag = currentRole.RoleTag,
                    DataPermission = currentRole.DataPermission,
                    EmloyeeCount = currentRole.Name == "Guest" ? -1 : Context.SysRoleEmployees.AsNoTracking().Count(x => x.RoleId == currentRole.Id)
                    
                };

                var RoleEmployees =await Context.SysRoleEmployees.Where(x => x.RoleId == request.RoleId).ToListAsync();
                var roleemployees = new List<RoleEmployee>();
                foreach (var item in RoleEmployees)
                {
                    var currentEmployee = await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.EmployeeId);

                    var employeeRoleMenu = await Context.SysRoleEmployeeMenu.AsNoTracking().AnyAsync(x => x.EmployeeId == item.EmployeeId);

                    var employeeReportTemplate = await Context.SysRoleEmployeeReportTemplate.AsNoTracking().AnyAsync(x => x.EmployeeId == item.EmployeeId);

                    bool hasReportData = false;


                    var employeeReportDepartment = await Context.SysRoleEmployeeReportDepartment.AsNoTracking().AnyAsync(x => x.EmployeeId == item.EmployeeId);

                    var employeeReportEmployer  = await Context.SysRoleEmployeeReportEmployer.AsNoTracking().AnyAsync(x => x.EmployeeId == item.EmployeeId);

                    if (employeeReportDepartment || employeeReportEmployer)
                    {
                        hasReportData = true;
                    }


                    var newData = new RoleEmployee
                    {
                        Id = item.Id,
                        AdAccount = currentEmployee?.ADAccount,
                        EmployeeId = currentEmployee?.Id,
                        Firstname = currentEmployee?.Firstname,
                        Lastname = currentEmployee?.Lastname,
                        LastLoginDate = item.LastLoginDate,
                        ReadonlyAccess = item.ReadonlyAccess,
                        HasMenu = employeeRoleMenu,
                        HasReport = employeeReportTemplate,
                        HasReportData = hasReportData,


                    };

                    roleemployees.Add(newData);
                }

                returnData.RoleUsers = roleemployees;

                return returnData;

            }
            else {
                return null;
            }

        }


        #region RoleInfo

        public async Task<GetEmployeeRoleInfoResponse> GetRoleInfoData(GetEmployeeRoleInfoRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GetEmployeeRoleInfoResponse();

            var RoleData =await Context.SysRoleEmployees.Where(x => x.EmployeeId == request.EmployeeId).FirstOrDefaultAsync();
            if (RoleData != null)
            {
                var currentRole =await Context.SysRole.AsNoTracking().Where(x => x.Id == RoleData.RoleId).FirstOrDefaultAsync();
                returnData.RoleName = currentRole?.Name;
                returnData.LastLoginDate = RoleData.LastLoginDate;
                returnData.Id = RoleData.Id;
            }
            else {
                returnData.RoleName = "Guest";
                returnData.Id =0;
            }


            returnData.GroupInfo = await (from groupData in Context.RequestGroupEmployee.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId)
                            join groupinfo in Context.RequestGroup.AsNoTracking() on groupData.RequestGroupId equals groupinfo.Id into groupinfoData
                            from groupinfo in groupinfoData.DefaultIfEmpty()
                            select new GetEmployeeGroupInfo
                            {
                                Id = groupData.Id,
                                GroupName = groupinfo.Description
                            }).ToListAsync();


            returnData.DepartmentAdminInfo = await (from departmentAdminData in Context.DepartmentAdmin.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId)
                                          join departmentinfo in Context.Department.AsNoTracking() on departmentAdminData.DepartmentId equals departmentinfo.Id into departmentinfoData
                                          from departmentinfo in departmentinfoData.DefaultIfEmpty()
                                          select new GetEmployeeDepartmentAdminInfo
                                          {
                                              Id = departmentAdminData.Id,
                                              DepartmentName = departmentinfo.Name
                                          }).ToListAsync();

            returnData.DepartmentManagerInfo = await (from departmentAdminData in Context.DepartmentManager.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId)
                                                    join departmentinfo in Context.Department.AsNoTracking() on departmentAdminData.DepartmentId equals departmentinfo.Id into departmentinfoData
                                                    from departmentinfo in departmentinfoData.DefaultIfEmpty()
                                                    select new GetEmployeeDepartmentManagerInfo
                                                    {
                                                        Id = departmentAdminData.Id,
                                                        DepartmentName = departmentinfo.Name
                                                    }).ToListAsync();

            returnData.DepartmentSupervisorInfo = await (from departmentSupervisorData in Context.DepartmentSupervisor.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId)
                                                      join departmentinfo in Context.Department.AsNoTracking() on departmentSupervisorData.DepartmentId equals departmentinfo.Id into departmentinfoData
                                                      from departmentinfo in departmentinfoData.DefaultIfEmpty()
                                                      select new GetEmployeeDepartmentSupervisorInfo
                                                      {
                                                          Id = departmentSupervisorData.Id,
                                                          DepartmentName = departmentinfo.Name
                                                      }).ToListAsync();

            if (RoleData != null)
            {
                if (RoleData.RoleId.HasValue)
                {
                    returnData.Menu = await GetEmployeeMenu(request.EmployeeId, RoleData.RoleId.Value, cancellationToken);
                }
                
            }
            


            return returnData;

        }

        #endregion


        private async Task<List<EmployeeMenu>> GetEmployeeMenu(int EmployeeId, int RoleId, CancellationToken cancellationToken)
        {

            var result = new List<EmployeeMenu>();


            var allMenu = await Context.SysMenu.AsNoTracking().ToListAsync(cancellationToken);
            var RoleMenuIds = await Context.SysRoleEmployeeMenu.AsNoTracking().Where(x => x.EmployeeId == EmployeeId).Select(x => new { x.MenuId }).ToListAsync(cancellationToken);

            if (RoleMenuIds.Count > 0)
            {

                foreach (var item in allMenu.OrderBy(x => x.Id))
                {
                    var itemMenuRole = RoleMenuIds.FirstOrDefault(x => x.MenuId == item.Id);
                    var newData = new EmployeeMenu
                    {
                        Id = item.Id,
                        Head_ID = item.ParentId == null ? 0 : item.ParentId,
                        Name = item.Name,
                        Code = item.Code,
                        Route = item.Route,
                        Permission = itemMenuRole != null ? 1 : 0
                    };

                    result.Add(newData);


                }


                return result;
            }



            return new List<EmployeeMenu>();


        }




    }

}
