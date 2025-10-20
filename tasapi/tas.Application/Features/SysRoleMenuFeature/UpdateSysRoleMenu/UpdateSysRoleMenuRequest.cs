using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysRoleMenuFeature.UpdateSysRoleMenu
{
    public sealed record UpdateSysRoleMenuRequest(int RoleId, List<MenuPermission> MenuPermissions) : IRequest;

    public sealed record MenuPermission {
        public int MenuId { get; set; }

        public int Permission { get; set; }
    }
}
