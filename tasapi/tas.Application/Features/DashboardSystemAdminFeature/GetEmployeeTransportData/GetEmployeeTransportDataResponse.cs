using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardSystemAdminFeature.GetEmployeeTransportData
{

    public sealed record GetEmployeeTransportDataResponse
    {
        
        public List<GetEmployeeTransportDataDirectionData> InData { get; set; }

        public List<GetEmployeeTransportDataDirectionData> OutData { get; set; }

    }


    public sealed record GetEmployeeTransportDataDirectionData
    {
        public DateTime? EventDate { get; set; }

        public string? Drilldown { get; set; }

        public string? Direction { get; set; }

        public int? Count { get; set; }

        public List<GetEmployeeTransportDataCommutBase> LocationData { get; set; }
    }


    public sealed record GetEmployeeTransportDataCommutBase
    {
        public DateTime? EventDate { get; set; }

        public string? Drilldown { get; set; }


        public string? Id { get; set; }

        public string? Direction { get; set; }

        public int? Count { get; set; }

        public string? Location { get; set; }

        public int? LocationId { get; set; }



        public List<GetEmployeeTransportDataState> StateData { get; set; }


    }



    public sealed record GetEmployeeTransportDataState
    {
        public DateTime? EventDate { get; set; }

        public string? Id { get; set; }

        public string? Direction { get; set; }

        public int? Count { get; set; }

        public string? StateDescr { get; set; }




    }






}
