using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysRoleEmployeeDashboardFeature.UpdateSysRoleEmployeeDashboard
{
    public sealed record UpdateSysRoleEmployeeDashboardRequest(int EmployeeId, List<EmployeeDashboardPermission> DashboardPermissions) : IRequest;

    public sealed record EmployeeDashboardPermission {
        public int DashboardId { get; set; }

        public int Permission { get; set; }
    }
}
