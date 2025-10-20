using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class SysRole : BaseEntity
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? DataPermission { get; set; }

        public string RoleTag { get; set; }

    }
}
