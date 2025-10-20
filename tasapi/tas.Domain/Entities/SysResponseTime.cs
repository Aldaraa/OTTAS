using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class SysResponseTime : BaseEntity
    {
        public string? Path { get; set; }
        public int? ResponseTime { get; set; }
    }
}
