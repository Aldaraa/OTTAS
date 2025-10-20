using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class ActiveTransport : BaseEntity
    {
        public string Code { get; set; }
        public string DayNum { get; set; }
        public string Direction { get; set; }
        public int CarrierId { get; set; }
        public int TransportModeId { get; set; }
        public int? TransportAudit { get; set; }
        public int? Seats { get; set; }
        public int? fromLocationId { get; set; }
        public int? toLocationId { get; set; }

        public int Special { get; set; } = 0;

        public int FrequencyWeeks { get; set; } = 1;

        public int? CostCodeId { get; set; }

        public string? Description { get; set; }

        public string? ETD { get; set; }
        public string? ETA { get; set; }

        public string? AircraftCode { get; set; }




        //public Carrier Carrier { get; set; }
        //public Location FromLocation { get; set; }
        //public Location ToLocation { get; set; }
        //public TransportMode TransportMode { get; set; }
    }
}
