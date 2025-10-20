using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{


    public sealed class SysMenu : BaseEntity
    {
        public string? Name { get; set; }
        public string? Code { get; set; }

        public string? Route { get; set; }

        public int? OrderIndex { get; set; }

        public int? ParentId { get; set; }

    }
}
