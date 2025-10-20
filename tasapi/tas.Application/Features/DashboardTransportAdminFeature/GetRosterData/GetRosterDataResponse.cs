using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardTransportAdminFeature.GetRosterData
{

    public sealed record GetRosterDataResponse
    {
        public List<GetRosterGroupData> RosterData { get; set; }

      //  public List<GetFlightGroupData> FlightGroupData { get; set; }



    }


    //public sealed record GetFlightGroupData
    //{

    //    public int? Count { get; set; }

    //    public string? Description { get; set; }

    //    public string? DayName { get; set; }





    //}

    public sealed record GetRosterGroupData
    {

        public int? Count { get; set; }

        public string? Description { get; set; }
        public int? Drilldown { get; set; }

        public List<GetRosterGroupDetail> Details { get; set; }



    }



    public sealed record GetRosterGroupDetail
    {
        public string? Id { get; set; }

        public int? Count { get; set; }

        public string? Name { get; set; }


    }



}
