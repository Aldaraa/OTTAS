using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.TransportScheduleFeature.SeatInfoTransportSchedule
{

    public sealed record SeatInfoTransportScheduleResponse 
    {
        public int? Seats { get; set; }

        public int? BookedCount { get; set; }

        public int? AvailableSeatCount { get; set; }
    }



}
