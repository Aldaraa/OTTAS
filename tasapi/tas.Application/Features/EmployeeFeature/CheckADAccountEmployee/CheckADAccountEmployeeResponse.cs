using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetEmployee;

namespace tas.Application.Features.EmployeeFeature.CheckADAccountEmployee
{
    public sealed record CheckADAccountEmployeeResponse
    {
        public bool AdAccountValidationStatus { get; set; }

        public string? AdAccountValidationFailedReason { get; set; }

        public string? Lastname { get; set; }

        public string? Firstname { get; set; }

        public int? EmployeeId { get; set; }
    }

   


}
