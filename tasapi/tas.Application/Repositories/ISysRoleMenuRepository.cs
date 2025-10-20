using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysRoleMenuFeature.GetAllSysRoleMenu;
using tas.Application.Features.SysRoleMenuFeature.UpdateSysRoleMenu;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{


     public  interface ISysRoleMenuRepository : IBaseRepository<SysRoleMenu>
     {
        Task UpdateMenuRole(UpdateSysRoleMenuRequest request, CancellationToken cancellationToken);

        Task<List<GetAllSysRoleMenuResponse>> GetRoleMenu(GetAllSysRoleMenuRequest request, CancellationToken cancellationToken);
     }
}
