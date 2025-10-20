using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.GetParentDepartments
{
    public sealed record GetParentDepartmentsResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? DepartmentLevel { get; set; }

        public string? AdminName { get; set; }
        public string? ManagerName { get; set; }
        public string? SupervisorName { get; set; }
        public int? AdminId { get; set; }
        public int? ManagerId { get; set; }
        public int? SupervisorId { get; set; }
    }

   
}
