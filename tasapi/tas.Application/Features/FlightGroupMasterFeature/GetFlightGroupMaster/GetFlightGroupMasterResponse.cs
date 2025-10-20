using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.FlightGroupMasterFeature.GetFlightGroupMaster
{
    public sealed record GetFlightGroupMasterResponse
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        public string? Description { get; set; }
        public int Active { get; set; }


        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public List<FlightGroupDetailResponseMaster> Detail { get; set; }
    }

    public sealed record FlightGroupDetailResponseMaster 
    {
        public int Id { get; set; }
        public int FlightGroupMasterId { get; set; }

        public int ShiftId { get; set; }

        public string? ShiftCode { get; set; }

        public int? ClusterId { get; set; }

        public string? Direction { get; set; }

        public string? DayNum { get; set; }

        public int? SeqNumber { get; set; }

    }
}
