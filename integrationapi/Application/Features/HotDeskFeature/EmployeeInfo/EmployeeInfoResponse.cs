
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.EmployeeInfo
{
    [Keyless]
    public sealed record EmployeeInfoResponse
    {

        public int EmployeeId { get; set; }

        public string Lastname { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string Email { get; set; }

        public int? SAPID { get; set; }

        public string? ADAccount { get; set; }

        public string? DepartmentName { get; set; }

        public int? DepartmentId { get; set; }

        public string? EmployerName { get; set; }

        public string? PositionName { get; set; }

        public int? PeopleTypeId { get; set; }

        public string? PeopleTypeName { get; set; }

        public int? Active { get; set; }

    }


}
