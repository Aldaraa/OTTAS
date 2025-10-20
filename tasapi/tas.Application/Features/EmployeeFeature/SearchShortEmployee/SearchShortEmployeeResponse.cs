using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Domain.Common;

namespace tas.Application.Features.EmployeeFeature.SearchShortEmployee
{

    public sealed record SearchShortEmployeeResponse : BasePaginationResponse<EmployeeSearchShortResult>
    {

    }

    public sealed class EmployeeSearchShortResult
    {
        public int Id { get; set; }
        public string? Lastname { get; set; }
        public string? Firstname { get; set; }
        public string? MFirstname { get; set; }
        public string? MLastname { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? NRN { get; set; }
        public int? SAPID { get; set; }


    }
}
