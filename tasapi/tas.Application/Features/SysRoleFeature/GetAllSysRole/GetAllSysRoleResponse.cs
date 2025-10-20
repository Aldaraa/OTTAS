using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysRoleFeature.GetAllSysRole
{
    public sealed record GetAllSysRoleResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public int? DataPermission { get; set; }

        public int? EmployeeCount { get; set; }

    }

}