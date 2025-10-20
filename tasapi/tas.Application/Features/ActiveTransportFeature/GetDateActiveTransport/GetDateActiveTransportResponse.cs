using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.ActiveTransportFeature.GetDateActiveTransport
{

    public sealed record GetDateActiveTransportResponse 
    {
        public int ActiveTransportId { get; set; }

        public string? Description { get; set; }

        public int ScheduleId { get; set; }

        public int? Seat { get; set; }

        public int? BookedCount { get; set; }



    }




}
