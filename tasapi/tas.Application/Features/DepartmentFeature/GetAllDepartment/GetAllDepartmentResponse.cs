using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.GetAllDepartment
{
    public sealed record GetAllDepartmentResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ParentDepartmentId { get; set; }
        public int Active { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public int EmployeeCount { get; set; }

        public int? Admins { get; set; }

        public int? Managers { get; set; }


        public int? Supervisers { get; set; }


        public int? CostCodeId { get; set; }


        public string? CostCodeDescr { get; set; }





        public ICollection<GetAllDepartmentResponse>? ChildDepartments { get; set; }

    }

   
}
