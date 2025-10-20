using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.SearchTransportSchedule
{

    public sealed record SearchTransportScheduleResponse
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        public int Active { get; set; }


        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
