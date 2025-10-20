using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardDataAdminFeature.GetEmployeeRegisterData
{

    public sealed record GetEmployeeRegisterDataResponse
    {
        public string? Description { get; set; }

        public int Cnt { get; set; }

    }




}
