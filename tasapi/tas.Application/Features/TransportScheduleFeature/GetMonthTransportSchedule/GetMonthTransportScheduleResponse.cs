using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.GetMonthTransportSchedule
{

    public sealed record GetMonthTransportScheduleResponse
    {
        public int Id { get; set; }
        public string? Code { get; set; }


        public string? Description { get; set; }

        public DateTime? EventDate { get; set; }


        public DateTime? EventDateETD { get; set; }

        public DateTime? EventDateETA { get; set; }

        public string? Direction { get; set; }

        public int? Seats { get; set; }

   //     public int? OvertBooked { get; set; }

        public int? Confirmed { get; set; }




    }
}
