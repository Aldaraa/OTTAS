using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysRoleEmployeeMenuFeature.GetAllSysRoleEmployeeMenu;
using tas.Application.Features.SysRoleEmployeeMenuFeature.UpdateSysRoleEmployeeMenu;
using tas.Application.Features.SysRoleEmployeeReportTemplateFeature.GetAllSysRoleEmployeeReportTemplate;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class SysRoleEmployeeMenuRepository : BaseRepository<SysRoleEmployeeMenu>, ISysRoleEmployeeMenuRepository
    {
        private readonly IConfiguration _Configuration;
        private readonly HTTPUserRepository _userRepository;
        private readonly SignalrHub _signalrHub;
        public SysRoleEmployeeMenuRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, SignalrHub signalrHub) : base(context, configuration, hTTPUserRepository)
        {
            _Configuration = configuration;
            _userRepository = hTTPUserRepository;
            _signalrHub = signalrHub;
        }

        #region RoleMenu Data


        public async Task<List<GetAllSysRoleEmployeeMenuResponse>> GetRoleMenu(GetAllSysRoleEmployeeMenuRequest request, CancellationToken cancellationToken)
        {


            var result = new List<GetAllSysRoleEmployeeMenuResponse>();

            var allMenu = await Context.SysMenu.AsNoTracking().ToListAsync(cancellationToken);
            var RoleEmployeeMenuIds = await Context.SysRoleEmployeeMenu.Where(x => x.EmployeeId == request.EmployeeId).Select(x => new { x.MenuId }).ToListAsync(cancellationToken);

            if (RoleEmployeeMenuIds.Count > 0)
            {
                foreach (var item in allMenu.OrderBy(x => x.Id))
                {
                    var itemMenuRole = RoleEmployeeMenuIds.FirstOrDefault(x => x.MenuId == item.Id);
                    var newData = new GetAllSysRoleEmployeeMenuResponse
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
            else
            {
                var currentRoleData = await Context.SysRoleEmployees.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId).FirstOrDefaultAsync();
                if (currentRoleData != null)
                {
                    var RoleMenuIds = await Context.SysRoleMenu.AsNoTracking().Where(x => x.RoleId == currentRoleData.RoleId).Select(x => new { x.MenuId }).ToListAsync(cancellationToken);

                    foreach (var item in allMenu.OrderBy(x => x.Id))
                    {
                        var itemMenuRole = RoleMenuIds.FirstOrDefault(x => x.MenuId == item.Id);
                        var newData = new GetAllSysRoleEmployeeMenuResponse
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
                else
                {
                    return new List<GetAllSysRoleEmployeeMenuResponse>();
                }
            }
            


        }

        #endregion

        public async Task UpdateMenuRole(UpdateSysRoleEmployeeMenuRequest request, CancellationToken cancellationToken)
        {

            var menuIds = request.MenuPermissions.Select(item => item.MenuId).ToList();

            var roleDeleteMenuIds = await Context.SysRoleEmployeeMenu
                .Where(x => x.EmployeeId == request.EmployeeId && !menuIds.Contains(x.MenuId))
                .ToListAsync(cancellationToken);

            foreach (var item in roleDeleteMenuIds)
            {
                Context.SysRoleEmployeeMenu.Remove(item);
            }

            foreach (var item in menuIds)
            {
                var existingMenu = await Context.SysRoleEmployeeMenu
                    .FirstOrDefaultAsync(x => x.EmployeeId == request.EmployeeId && x.MenuId == item);

                if (existingMenu == null)
                {
                    Context.SysRoleEmployeeMenu.Add(new SysRoleEmployeeMenu
                    {
                        Active = 1,
                        DateCreated = DateTime.Now,
                        MenuId = item,
                        EmployeeId = request.EmployeeId,
                        UserIdCreated = _userRepository.LogCurrentUser()?.Id
                    });
                }
            }

            var currentEmployee = await Context.Employee.AsNoTracking().Where(x=> x.Id == request.EmployeeId).FirstOrDefaultAsync(cancellationToken);
            if (currentEmployee != null) {

                if (!string.IsNullOrWhiteSpace(currentEmployee.ADAccount)) {
                    _userRepository.ClearRoleCache(currentEmployee.ADAccount);
                }
                
            }

            await _signalrHub.RoleChange(Convert.ToString(request.EmployeeId));

        }


        }
    }
