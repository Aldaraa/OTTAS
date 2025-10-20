using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.GetAllReportDepartment
{
    public sealed record GetAllReportDepartmentResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ParentDepartmentId { get; set; }





        public ICollection<GetAllReportDepartmentResponse>? ChildDepartments { get; set; }

    }

   
}
