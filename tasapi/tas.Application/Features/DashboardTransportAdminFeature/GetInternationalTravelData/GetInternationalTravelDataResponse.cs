using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardTransportAdminFeature.GetInternationalTravelData
{

    public sealed record GetInternationalTravelDataResponse
    {
        public List<GetInternationalTravelDataHotel> Hotel { get; set; }

        public List<GetInternationalTravelDataTravelPurpose>? TravelPurpose { get; set; }

        public List<GetInternationalTravelDataTravelAgent>? TravelAgent { get; set; }


        public List<GetInternationalTravelDataDocument>? Documents { get; set; }

    }


    public sealed record GetInternationalTravelDataTravelPurpose
    {
        public string? TravelPurpose { get; set; }

        public int Count { get; set; }


    }

    public sealed record GetInternationalTravelDataHotel
    {
        public string? Hotel { get; set; }


        public int? Count { get; set; }




    }

    public sealed record GetInternationalTravelDataTravelAgent
    {
        public string? Agent { get; set; }

        public int? Count { get; set; }



    }

    public sealed record GetInternationalTravelDataDocument
    {
        public int? Count { get; set; }

        public string? Action { get; set; }


    }




}
