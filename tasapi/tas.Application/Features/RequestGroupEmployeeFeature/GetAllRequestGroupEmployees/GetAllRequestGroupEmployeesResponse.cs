using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupEmployeeFeature.GetAllRequestGroupEmployees
{

    public sealed record GetAllRequestGroupEmployeesResponse
    {
        public int EmployeeId { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public int? TaskCount { get; set; }



    }

}
