using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.AuthenticationFeature.LoginUser;
using tas.Application.Features.SysRoleEmployeeDashboardFeature.GetAllSysRoleEmployeeDashboard;
using tas.Application.Features.SysRoleEmployeeDashboardFeature.UpdateSysRoleEmployeeDashboard;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class SysRoleEmployeeDashboardRepository : BaseRepository<SysRoleEmployeeDashboard>, ISysRoleEmployeeDashboardRepository
    {
        private readonly IConfiguration _Configuration;
        private readonly HTTPUserRepository _userRepository;
        private readonly SignalrHub _signalrHub;
        public SysRoleEmployeeDashboardRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, SignalrHub signalrHub) : base(context, configuration, hTTPUserRepository)
        {
            _Configuration = configuration;
            _userRepository = hTTPUserRepository;
            _signalrHub = signalrHub;
        }

        #region RoleDashboard Data


        public async Task<List<GetAllSysRoleEmployeeDashboardResponse>> GetRoleDashboard(GetAllSysRoleEmployeeDashboardRequest request, CancellationToken cancellationToken)
        {


            var result = new List<GetAllSysRoleEmployeeDashboardResponse>();

            var userRoleData = await Context.SysRoleEmployees.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId).FirstOrDefaultAsync();
            if (userRoleData != null)
            {

                var userRole = await Context.SysRole.Where(x => x.Id == userRoleData.RoleId).FirstOrDefaultAsync();
                if (userRole != null)
                {

                    var allDashboard = await Context.SysDashboard.AsNoTracking().ToListAsync(cancellationToken);
                    var RoleEmployeeDashboardIds = await Context.SysRoleEmployeeDashboard.Where(x => x.EmployeeId == request.EmployeeId).Select(x => new { x.DashboardId }).ToListAsync(cancellationToken);

                    if (RoleEmployeeDashboardIds.Count > 0)
                    {
                        foreach (var item in allDashboard.OrderBy(x => x.Id))
                        {
                            var itemDashboardRole = RoleEmployeeDashboardIds.FirstOrDefault(x => x.DashboardId == item.Id);
                            var newData = new GetAllSysRoleEmployeeDashboardResponse
                            {
                                Id = item.Id,
                                Name = item.Name,
                                Code = item.Code,
                                Permission = itemDashboardRole != null ? 1 : 0
                            };

                            result.Add(newData);
                        }
                        return result;
                    }
                    else
                    {

                        if (userRole.RoleTag == "SystemAdmin")
                        {

                            var sysDashboards = await Context.SysDashboard.AsNoTracking().Select(x => new GetAllSysRoleEmployeeDashboardResponse
                            {
                                Code = x.Code,
                                Name = x.Name,
                                Id = x.Id,
                                Permission = 1
                            }).ToListAsync();

                            return sysDashboards;

                        }
                        else if (userRole.RoleTag == "AccomAdmin")
                        {
                            var sysDashboards = await Context.SysDashboard.AsNoTracking().Select(x => new GetAllSysRoleEmployeeDashboardResponse
                            {
                                Code = x.Code,
                                Name = x.Name,
                                Id = x.Id,
                                Permission = x.Code == "DASHBOARD_103" ? 1 : 0
                            }).ToListAsync();

                            return sysDashboards;
                        }
                        else if (userRole.RoleTag == "TravelAdmin")
                        {
                            var sysDashboards = await Context.SysDashboard.AsNoTracking().Select(x => new GetAllSysRoleEmployeeDashboardResponse
                            {
                                Code = x.Code,
                                Name = x.Name,
                                Id = x.Id,
                                Permission = x.Code == "DASHBOARD_104" ? 1 : 0
                            }).ToListAsync();

                            return sysDashboards;
                        }
                        else if (userRole.RoleTag == "DataApproval")
                        {
                            var sysDashboards = await Context.SysDashboard.AsNoTracking().Select(x => new GetAllSysRoleEmployeeDashboardResponse
                            {
                                Code = x.Code,
                                Name = x.Name,
                                Id = x.Id,
                                Permission = x.Code == "DASHBOARD_102" ? 1 : 0
                            }).ToListAsync();

                            return sysDashboards;
                        }
                        else
                        {
                            var sysDashboards = await Context.SysDashboard.AsNoTracking().Select(x => new GetAllSysRoleEmployeeDashboardResponse
                            {
                                Code = x.Code,
                                Name = x.Name,
                                Id = x.Id,
                                Permission = 0
                            }).ToListAsync();

                            return sysDashboards;
                        }

                    }

                }
                else
                {
                    return result;
                }

            }
            else { 
            return result;
            }



        }

        #endregion

        public async Task UpdateDashboardRole(UpdateSysRoleEmployeeDashboardRequest request, CancellationToken cancellationToken)
        {

            var DashboardIds = request.DashboardPermissions.Select(item => item.DashboardId).ToList();

            var roleDeleteDashboardIds = await Context.SysRoleEmployeeDashboard
                .Where(x => x.EmployeeId == request.EmployeeId && !DashboardIds.Contains(x.DashboardId))
                .ToListAsync(cancellationToken);

            foreach (var item in roleDeleteDashboardIds)
            {
                Context.SysRoleEmployeeDashboard.Remove(item);
            }

            foreach (var item in DashboardIds)
            {
                var existingDashboard = await Context.SysRoleEmployeeDashboard
                    .FirstOrDefaultAsync(x => x.EmployeeId == request.EmployeeId && x.DashboardId == item);

                if (existingDashboard == null)
                {
                    Context.SysRoleEmployeeDashboard.Add(new SysRoleEmployeeDashboard
                    {
                        Active = 1,
                        DateCreated = DateTime.Now,
                        DashboardId = item,
                        EmployeeId = request.EmployeeId,
                        UserIdCreated = _userRepository.LogCurrentUser()?.Id
                    });
                }
            }

            var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync(cancellationToken);
            if (currentEmployee != null)
            {

                if (!string.IsNullOrWhiteSpace(currentEmployee.ADAccount))
                {
                    _userRepository.ClearRoleCache(currentEmployee.ADAccount);
                }

            }

            await _signalrHub.RoleChange(Convert.ToString(request.EmployeeId));

        }


    }
}
