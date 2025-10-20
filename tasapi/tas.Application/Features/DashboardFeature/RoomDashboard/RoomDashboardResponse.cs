using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.DashboardFeature.RoomDashboard
{
    public sealed record RoomDashboardResponse
    {

        public int? TotalRooms { get; set; }

        public int? TodayActiveBed { get; set; }

        public int? TodayEmptyRoom { get; set; }

        public int TodayVirtualRoomEmloyees { get; set; }

        public List<VirtualRoomWeekData> VirtualRoomWeek { get; set; }

        public List<ActiveRoomWeekData> ActiveRoomWeek { get; set; }

        public List<EmptyRoomWeekData> EmptyRoomWeek { get; set; }


    }

    public sealed record VirtualRoomWeekData
    {
        public DateTime date { get; set; }

        public int VirtualRoomEmployeeCount { get; set; }


    }

    public sealed record ActiveRoomWeekData
    {
        public DateTime date { get; set; }

        public int? ActiveRoomCount { get; set; }


    }

    public sealed record EmptyRoomWeekData
    {
        public DateTime date { get; set; }

        public int EmptyRoomCount { get; set; }


    }

}
