using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{

    public sealed class Location : BaseEntity
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int onSite { get; set; } = 0;

    }
}
