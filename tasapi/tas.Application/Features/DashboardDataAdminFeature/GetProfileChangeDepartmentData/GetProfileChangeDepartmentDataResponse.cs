using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardDataAdminFeature.GetProfileChangeDepartmentData
{

    public sealed record GetProfileChangeDepartmentDataResponse
    {

        public List<GetProfileChangeEditDataDate> EditFields { get; set; }

    }




    public sealed record GetProfileChangeEditDataDate
    {
        public string? ColumnName { get; set; }

        public int? Cnt { get; set; }

    }

}
