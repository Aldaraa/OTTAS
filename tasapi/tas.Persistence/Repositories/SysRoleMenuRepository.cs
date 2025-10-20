using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysMenuFeature.GetAllMenu;
using tas.Application.Features.SysRoleMenuFeature.GetAllSysRoleMenu;
using tas.Application.Features.SysRoleMenuFeature.UpdateSysRoleMenu;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class SysRoleMenuRepository : BaseRepository<SysRoleMenu>, ISysRoleMenuRepository
    {
        private readonly IConfiguration _Configuration;
        private readonly HTTPUserRepository _userRepository;
        public SysRoleMenuRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _Configuration = configuration;
            _userRepository = hTTPUserRepository;
        }

        #region RoleMenu Data


        public async Task<List<GetAllSysRoleMenuResponse>> GetRoleMenu(GetAllSysRoleMenuRequest request, CancellationToken cancellationToken)
        {
  

            var result = new List<GetAllSysRoleMenuResponse>();

            var allMenu = await Context.SysMenu.ToListAsync(cancellationToken);
            var RoleMenuIds =await Context.SysRoleMenu.Where(x => x.RoleId == request.RoleId).Select(x => new { x.MenuId }).ToListAsync(cancellationToken);

            foreach (var item in allMenu.OrderBy(x=> x.Id))
            {
                var itemMenuRole = RoleMenuIds.FirstOrDefault(x => x.MenuId == item.Id);
                var newData = new GetAllSysRoleMenuResponse
                { 
                    Id = item.Id,
                    Head_ID = item.ParentId == null ? 0 : item.ParentId,
                    Name = item.Name, 
                    Code = item.Code,
                    Route = item.Route,
                    Permission = itemMenuRole != null ? 1  : 0
                };

                result.Add(newData);


            }
            return result;
        }

        #endregion

        public async Task UpdateMenuRole(UpdateSysRoleMenuRequest request, CancellationToken cancellationToken)
        {

            var menuIds = request.MenuPermissions.Select(item => item.MenuId).ToList();

            var roleDeleteMenuIds = await Context.SysRoleMenu
                .Where(x => x.RoleId == request.RoleId && !menuIds.Contains(x.MenuId))
                .ToListAsync(cancellationToken);

            foreach (var item in roleDeleteMenuIds)
            {
                Context.SysRoleMenu.Remove(item);
            }

            foreach (var item in menuIds)
            {
                var existingMenu = await Context.SysRoleMenu
                    .FirstOrDefaultAsync(x => x.RoleId == request.RoleId && x.MenuId == item);

                if (existingMenu == null)
                {
                    Context.SysRoleMenu.Add(new SysRoleMenu
                    {
                        Active = 1,
                        DateCreated = DateTime.Now,
                        MenuId = item,
                        RoleId = request.RoleId,
                        UserIdCreated = _userRepository.LogCurrentUser()?.Id
                    });
                }
            }

            await Context.SaveChangesAsync(cancellationToken);



            var employeeIds = await  Context.SysRoleEmployees.Where(c=> c.RoleId == request.RoleId).Select(c=> c.EmployeeId).ToListAsync(cancellationToken); 

           var employees = await Context.Employee.Where(x=> employeeIds.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var item in employees)
            {
                if (!string.IsNullOrWhiteSpace(item.ADAccount)) 
                {
                    _userRepository.ClearRoleCache(item.ADAccount);
                }
                
            }


      
        }

    }
}
