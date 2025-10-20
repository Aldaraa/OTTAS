using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupEmployeeFeature.GetRequestGroupEmployees
{

    public sealed record GetRequestGroupEmployeesResponse
    { 
        public int Id { get; set; }

        public string Name { get; set; }

        public string? GroupTag { get; set; }

        public List<GetRequestGroupEmployees> Employees { get; set; }
    }

    public sealed record GetRequestGroupEmployees
    {
        public int Id { get; set; }
    
        public int EmployeeId { get; set; }
        public string? DisplayName { get; set; }

        public string? FullName { get; set; } 

        public int? PrimaryContact { get; set; }

        public string? ADAccount { get; set; }

        public int OrderIndex { get; set; }

        public int? SAPID { get; set; }




    }
}
