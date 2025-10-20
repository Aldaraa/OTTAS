using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.TransportScheduleInfo
{
 
    public sealed record TransportScheduleInfoResponse
    {
        public int Id { get; set; }
        public DateTime? EventDate { get; set; }
        public int? Seats { get; set; }

        public string? ETD { get; set; }

        public string? ETA { get; set; }
        public string? TransportCode { get; set; }

        public int? CarrierId { get; set; }

        public int TransportModeId { get; set; }



    }
}
