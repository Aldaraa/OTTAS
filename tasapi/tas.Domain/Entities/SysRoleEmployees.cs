using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class SysRoleEmployees : BaseEntity
    {
        public int? RoleId { get; set; }

        public int? EmployeeId { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public int? ReadonlyAccess { get; set; } = 0;

        public int? Agreement { get; set; } = 0;


    }
}
