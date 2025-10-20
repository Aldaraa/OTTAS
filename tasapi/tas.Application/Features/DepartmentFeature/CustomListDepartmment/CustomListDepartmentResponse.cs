using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.CustomListDepartmment
{
    public sealed record CustomListDepartmentResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? parentdepartmentname { get; set; }

        public int? EmployeeCount { get; set; }
    }
}
