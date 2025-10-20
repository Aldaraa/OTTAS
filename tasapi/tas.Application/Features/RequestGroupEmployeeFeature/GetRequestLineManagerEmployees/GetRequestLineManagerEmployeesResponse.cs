using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupEmployeeFeature.GetRequestLineManagerEmployees
{

    public sealed record GetRequestLineManagerEmployeesResponse
    { 
        public int Id { get; set; }

        public string Name { get; set; }

        public List<GetRequestLineManagerEmployees> Employees { get; set; }
    }

    public sealed record GetRequestLineManagerEmployees
    {
        public int Id { get; set; }
    
        public int EmployeeId { get; set; }
        public string? DisplayName { get; set; }

        public string? FullName { get; set; } 

        public int? PrimaryContact { get; set; }

        public int OrderIndex { get; set; }


        public int? SAPID { get; set; }

        public string? ADAccount { get; set; }

    }
}
