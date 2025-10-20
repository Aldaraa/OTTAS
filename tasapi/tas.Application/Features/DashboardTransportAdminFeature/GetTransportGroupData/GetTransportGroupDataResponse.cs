using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardTransportAdminFeature.GetTransportGroupData
{

    public sealed record GetTransportGroupDataResponse
    {
        public int? Id { get; set; }
        public int? Count { get; set; }

        public string? Description { get; set; }

        public string? DayName { get; set; }

    }


}
