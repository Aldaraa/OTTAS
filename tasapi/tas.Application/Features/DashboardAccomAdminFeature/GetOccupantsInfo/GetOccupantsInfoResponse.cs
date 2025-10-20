using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardAccomAdminFeature.GetOccupantsInfo
{

    public sealed record GetOccupantsInfoResponse
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int WeekNumber { get; set; }
        public List<GetOccupantsInfoByBedCount> ByBedCountData { get; set; }

        public List<GetOccupantsInfoCount> AllData { get; set; }


    }

    public sealed record GetOccupantsInfoByBedCount
    {
        public string? OccupantType { get; set; }

        public string? RoomType { get; set; }

        public int? RoomCount { get; set; }
        public int TotalOccupants { get; set; }
    }


    public sealed record GetOccupantsInfoCount
    {
        public string? Code { get; set; }


        public int? Count { get; set; }

        public string? EventDate { get; set; }


    }




}
