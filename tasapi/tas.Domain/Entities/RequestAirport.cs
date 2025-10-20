using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestAirport : BaseEntity
    {
        public string Code { get; set; }
        
        public string Description { get; set; }

        public string Country { get; set; }

        public int? OrderIndex { get; set; }

        public string? TimeZone { get; set; }
    }
}
