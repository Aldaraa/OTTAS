using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardDataAdminFeature.GetTransportData
{

    public sealed record GetTransportDataResponse
    {
        public List<GetTransportDataResponseTransport>? TransportData { get; set; }

        public List<GetTransportDataResponseTransportDetails>? Details { get; set; }




    }


    public sealed record GetTransportDataResponseTransport
    {
        public string? Type { get; set; }

        public List<GetTransportDataResponseTransportDate>? DateData { get; set; }


    }

    public sealed record GetTransportDataResponseTransportDate
    {
        public string? Key { get; set; }

        public string? Datekey { get; set; }

        public int? Cnt { get; set; }




    }

    public sealed record GetTransportDataResponseTransportDetails
    {
        public string? key { get; set; }

        public DateTime? EventDate { get; set; }

        public List<GetTransportDataResponseTransportDetailDate>? Data { get; set; }


    }

    public sealed record GetTransportDataResponseTransportDetailDate
    {
        public int? Cnt { get; set; }

        public string? Name { get; set; }


    }




}
