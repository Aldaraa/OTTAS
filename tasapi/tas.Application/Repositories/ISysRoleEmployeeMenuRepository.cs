using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysRoleEmployeeMenuFeature.GetAllSysRoleEmployeeMenu;
using tas.Application.Features.SysRoleEmployeeMenuFeature.UpdateSysRoleEmployeeMenu;
using tas.Application.Features.SysRoleMenuFeature.GetAllSysRoleMenu;
using tas.Application.Features.SysRoleMenuFeature.UpdateSysRoleMenu;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{


    public interface ISysRoleEmployeeMenuRepository : IBaseRepository<SysRoleEmployeeMenu>
    {
        Task UpdateMenuRole(UpdateSysRoleEmployeeMenuRequest request, CancellationToken cancellationToken);

        Task<List<GetAllSysRoleEmployeeMenuResponse>> GetRoleMenu(GetAllSysRoleEmployeeMenuRequest request, CancellationToken cancellationToken);
    }
}
