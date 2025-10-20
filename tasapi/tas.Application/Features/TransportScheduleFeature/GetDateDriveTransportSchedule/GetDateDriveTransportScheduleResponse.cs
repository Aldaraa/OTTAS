using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.TransportScheduleFeature.GetDateDriveTransportSchedule
{

    public sealed record GetDateDriveTransportScheduleResponse 
    {
        public int Id { get; set; }
        public string? Code { get; set; }


        public string? Description { get; set; }

        public DateTime? EventDate { get; set; }
        public DateTime? EventDateTime { get; set; }


        public int? Special { get; set; }


        public string? Direction { get; set; }
        public string? ETD { get; set; }
        public string? ETA { get; set; }

        public int? Seats { get; set; }


        public int? Confirmed { get; set; }

        public int? FromLocationId { get; set; }
        public int? ToLocationId { get; set; }
    }

  

}
