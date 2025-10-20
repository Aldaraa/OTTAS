using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysRoleEmployeeDashboardFeature.GetAllSysRoleEmployeeDashboard;
using tas.Application.Features.SysRoleEmployeeDashboardFeature.UpdateSysRoleEmployeeDashboard;
using tas.Application.Features.SysRoleEmployeeMenuFeature.GetAllSysRoleEmployeeMenu;
using tas.Application.Features.SysRoleEmployeeMenuFeature.UpdateSysRoleEmployeeMenu;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface ISysRoleEmployeeDashboardRepository : IBaseRepository<SysRoleEmployeeDashboard>
    {
        Task UpdateDashboardRole(UpdateSysRoleEmployeeDashboardRequest request, CancellationToken cancellationToken);

        Task<List<GetAllSysRoleEmployeeDashboardResponse>> GetRoleDashboard(GetAllSysRoleEmployeeDashboardRequest request, CancellationToken cancellationToken);
    }
}
