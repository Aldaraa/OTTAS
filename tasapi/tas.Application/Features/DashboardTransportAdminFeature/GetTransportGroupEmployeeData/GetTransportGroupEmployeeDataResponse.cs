using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardTransportAdminFeature.GetTransportGroupEmployeeData
{

    public sealed record GetTransportGroupEmployeeDataResponse
    {
        public int? Id { get; set; }

        public int EmployeeId { get; set; }

        public string? FullName { get; set; }

        public DateTime? EventDate { get; set; }

        public string? ActiveTransportCode { get; set; }
        public string? ScheduleCode { get; set; }

        public string? Status { get; set; }

        public string? DepartmentName { get; set; }

        public string? EmployerName { get; set; }

        public int? SAPID { get; set; }







    }


}
