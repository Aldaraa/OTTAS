using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardSystemAdminFeature.GetCampPOBData
{

    public sealed record GetCampPOBDataResponse
    {
        public string? Camp { get; set; }

        public string? RoomType { get; set; }


        public int? Count { get; set; }




    }




}
