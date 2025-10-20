using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestLineManagerEmployeeFeature.GetRequestLineManagerEmployee
{

    public sealed record GetRequestLineManagerEmployeeResponse
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public string? EmployeeFullName { get; set; }

        public int? LineManagerEmployeeId { get; set; }

        public string? LineManagerFullName { get; set; }




    }
   
}
