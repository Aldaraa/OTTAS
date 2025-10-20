using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardAccomAdminFeature.GetNonSiteInfo
{

    public sealed record GetNonSiteInfoResponse
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int WeekNumber { get; set; }
        public List<GetNonSiteInfoEmployees> Employees { get; set; }


    }


    public sealed record GetNonSiteInfoEmployees
    { 
        public string? PeopleType { get; set; }

        public int? OnSiteEmployee { get; set; }


    }




}
