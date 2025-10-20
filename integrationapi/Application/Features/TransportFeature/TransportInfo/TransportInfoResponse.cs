
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.TransportFeature.TransportInfo
{
    [Keyless]
    public sealed record TransportInfoResponse
    {
        public int? SAPID { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Direction { get; set; }

        public string? ScheduleDescription { get; set; }

        public string? TransportMode { get; set; }

        public string? RoomNumber { get; set; }

        public string? Carrier { get; set; }

    }




}
