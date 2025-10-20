using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardTransportAdminFeature.GetDomesticData
{

    public sealed record GetDomesticDataResponse
    {
        public GetDomesticAircraft AircraftData { get; set; }

        public List<GetDomesticDateTransport> DateTransport { get; set; }

        public List<GetDomesticDocument> Document { get; set; }

        public List<GetDomesticWeekUtil>WeekUtil { get; set; }

        public List<GetDomesticNoShow> NoShow { get; set; }


    }


    public sealed record GetDomesticAircraft
    {
        public List<GetDomesticAircraftData> Airplane { get; set; }

        public List<GetDomesticAircraftData> Bus { get; set; }

        public List<GetDomesticAircraftData> Drive { get; set; }

    }



 
    

    public sealed record GetDomesticAircraftData
    {
        public string? Code { get; set; }

        public int? ScheduleId { get; set; }

        public List<GetDomesticAircraftWeekInfo> WeekOUTInfo { get; set; }

        public List<GetDomesticAircraftWeekInfo> WeekINInfo { get; set; }



    }

    public sealed record GetDomesticAircraftWeekInfo
    { 
        public int? Count { get; set; }

        public string? Dayname  { get; set; }


    }







    public sealed record GetDomesticDateTransport
    {
        
        public int? IN { get; set; }

        public int? OUT { get; set; }
        public string? Code { get; set; }

    }


    public sealed record GetDomesticDocument
    {

        public int Count { get; set; }

        public string? Action { get; set; }
        


    }


    public sealed record GetDomesticWeekUtil
    {

        public int? Passengers { get; set; }

        public string? Carrier { get; set; }






    }


    public sealed record GetDomesticNoShow
    {

        public int Count { get; set; }

        public string? WeekNumber { get; set; }


        public string? Type { get; set; }




    }


}
