using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.TransportScheduleFeature.BusstopTransportSchedule
{

    public sealed record BusstopTransportScheduleResponse : BasePaginationResponse<BusstopTransportScheduleResult>
    {
    }

    public sealed class BusstopTransportScheduleResult
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        public string? TransportMode { get; set; }

        public string? Description { get; set; }

        public DateOnly? EventDate { get; set; }

        public int? Special { get; set; }

        public DateTime? EventDateETD { get; set; }

        public DateTime? EventDateETA { get; set; }

        public string? Direction { get; set; }

        public int? Seats { get; set; }

      //  public int? OvertBooked { get; set; }

        public int? Confirmed { get; set; }

        public int? FromLocationId { get; set; }
        public int? ToLocationId { get; set; }

        public bool BusstopStatus { get; set; }



    }


}
