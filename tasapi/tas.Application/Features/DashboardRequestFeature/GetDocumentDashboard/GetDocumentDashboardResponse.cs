
using MediatR;
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.Application.Features.DashboardRequestFeature.GetDocumentDashboard
{


    public sealed record GetDocumentDashboardResponse 
    {
        public List<GetDocumentDashboardRequestData> totalRequests { get; set; }

        public List<GetDocumentDashboardMonthData> monthData{ get; set; }

        public List<GetDocumentDashboardSiteTravelDeclined> SiteTravelDeclined { get; set; }

        public List<GetDocumentDashboardPendingDayAway> PendingDocumentDaysAway { get; set; }





    }

    public sealed record GetDocumentDashboardRequestData
    {
        public string? CurrentStatus { get; set; }
        public string? documentType { get; set; }

        public int count { get; set; }
    }


    public sealed record GetDocumentDashboardMonthData
    {
        public int day { get; set; }
        public int value { get; set; }

        public string? documentType { get; set; }
    }

    public sealed record GetDocumentDashboardSiteTravelDeclined
    { 
        public string? Comment { get; set; }

        public int? Count { get; set; }
        
    }



    public sealed record GetDocumentDashboardPendingDayAway
    {
        public string? DocumentType { get; set; }

        public int? DaysAway { get; set; }

        public int? Count { get; set; } 

    }



}