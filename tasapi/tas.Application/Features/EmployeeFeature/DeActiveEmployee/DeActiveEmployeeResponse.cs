using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetEmployee;

namespace tas.Application.Features.EmployeeFeature.DeActiveEmployee
{
    public sealed record DeActiveEmployeeResponse
    {
        public int EmployeeId { get; set; }

        public int? SAPID { get; set; }
        public string? FullName { get; set; }
    }

   


}
