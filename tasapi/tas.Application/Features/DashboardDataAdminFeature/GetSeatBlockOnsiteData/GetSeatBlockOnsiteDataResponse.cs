using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardDataAdminFeature.GetSeatBlockOnsiteData
{

    public sealed record GetSeatBlockOnsiteDataResponse
    {
        public string? DepartmentName { get; set; }

        public int? Cnt { get; set; }

        public DateTime? EventDate { get; set; }


    }




}
