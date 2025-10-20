using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardAccomAdminFeature.GetPobInfo
{

    public sealed record GetPobInfoResponse
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int WeekNumber { get; set; }
        public int POB { get; set; }

        public List<GetPobInfoDates> PobDates { get; set; }


    }


    public sealed record GetPobInfoDates
    { 
        public DateTime? EventDate { get; set; }

        public int? OnSiteEmployees { get; set; }


    }







}
