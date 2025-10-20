using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.DashboardFeature.TransportDashboard
{
    public sealed record TransportDashboardResponse
    {

        public int? TodayINTransport { get; set; }

        public int? TodayOUTTransport { get; set; }

        public int TodayINEmployeeCount { get; set; }

        public int TodayOUTEmployeeCount { get; set; }



        public List<SeatCountWeekData> SeatWeek { get; set; }



    }

    public sealed record SeatCountWeekData
    {
        public DateTime date { get; set; }

        public int? InEmloyeeCount { get; set; }

        public int? OutEmloyeeCount { get; set; }


        public int InConfirmed { get; set; }

        public int InOverBook { get; set; }

        public int InSeatBlock { get; set; }

        public int OutConfirmed { get; set; }

        public int OutOverBook { get; set; }

        public int OutSeatBlock { get; set; }



    }

  
}
