using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.EmployeeExistingTransport
{

    public sealed record EmployeeExistingTransportResponse
    {
        public int Id { get; set; }


        public string? Direction { get; set; }

        public DateTime? TravelDate { get; set; }


        public string? fromLocationCode { get; set; }


        public int? fromLocationId { get; set; }

        public string? toLocationCode { get; set; }

        public int? toLocationId { get; set; }

        public string? ScheduleCode { get; set; }

        public int? ScheduleId { get; set; }

        public string? ETA { get; set; }

        public string? ETD { get; set; }


        public string? status { get; set; }


    }
}
