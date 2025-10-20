using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupEmployeeFeature.GetRequestLineManagerAdminEmployees
{

    public sealed record GetRequestLineManagerAdminEmployeesResponse
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
