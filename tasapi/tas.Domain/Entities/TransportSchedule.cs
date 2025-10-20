using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class TransportSchedule : BaseEntity
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public int? Seats { get; set; }
        public string? ETD { get; set; }
        public string? ETA { get; set; } 
        public int ActiveTransportId { get; set; }
        public string? RealETD { get; set; }

        public string? Remark { get; set; }

    }
}
