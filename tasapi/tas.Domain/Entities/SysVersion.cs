using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public class SysVersion : BaseEntity
    {
        public string? Version { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public string? ReleaseNote { get; set; }
    }
}
