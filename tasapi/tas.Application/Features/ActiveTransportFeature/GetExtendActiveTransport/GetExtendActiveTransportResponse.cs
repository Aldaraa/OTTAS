using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.ActiveTransportFeature.GetExtendActiveTransport
{

    public sealed record GetExtendActiveTransportResponse 
    {
        public int Id { get; set; }

        public string? Code { get; set; }

        public string? DayNum { get; set; }

        public int? Seat { get; set; }

        public string? ETA { get; set; }

        public string? ETD { get; set; }


        public string? FromLocation { get; set; }

        public string? ToLocation { get; set; }

        public string? TransportMode { get; set; }

        public string? Carrier { get; set; }


        public DateTime? ScheduleStartDate { get; set; }


        public int? FrequencyWeeks { get; set; }






    }




}
