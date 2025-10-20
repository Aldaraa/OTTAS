using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysRoleEmployeeMenuFeature.UpdateSysRoleEmployeeMenu
{
    public sealed record UpdateSysRoleEmployeeMenuRequest(int EmployeeId, List<EmployeeMenuPermission> MenuPermissions) : IRequest;

    public sealed record EmployeeMenuPermission {
        public int MenuId { get; set; }

        public int Permission { get; set; }
    }
}
