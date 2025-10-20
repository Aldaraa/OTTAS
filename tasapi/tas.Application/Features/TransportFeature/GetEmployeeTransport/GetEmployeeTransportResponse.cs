using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.GetEmployeeTransport
{
 
    public sealed record GetEmployeeTransportResponse
    {
        public int Id { get; set; }
        public DateTime? EventDate { get; set; }

        public string? Status { get; set; }

        public string? Code { get; set; }

        public string? Description { get; set; }

        public int? ScheduleId { get; set; }

        public int? toLocationId { get; set; }
        public int? fromLocationId { get; set; }

        public string? Direction { get; set; }


        public string? TransportMode { get; set; }

    }
}
