using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardSystemAdminFeature.GetCampUtilizationData
{

    public sealed record GetCampUtilizationDataResponse
    {
        public int? CampId { get; set; }
        public string? Camp { get; set; }

        public int? RoomCount { get; set; }


        public int? BedCount { get; set; }

        public int? Occup { get; set; }


        public int? Utilization { get; set; }




    }




}
