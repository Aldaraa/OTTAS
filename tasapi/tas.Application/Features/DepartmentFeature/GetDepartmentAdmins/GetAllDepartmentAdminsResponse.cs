using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.GetAllDepartmentAdmins
{
    public sealed record GetAllDepartmentAdminsResponse
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public string? FullName { get; set; }
       
        public int? SAPID { get; set; }

        public int? DepartmentId { get; set; }

        public string? DepartmentName { get; set; }
    }

   
}
