using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardSystemAdminFeature.GetStatData
{

    public sealed record GetStatDataResponse
    {
        public int? Count { get; set; }


        public string? Code { get; set; }

        public string? Description { get; set; }





    }





}
