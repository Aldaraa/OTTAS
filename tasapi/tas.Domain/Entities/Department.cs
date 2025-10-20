using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class Department : BaseEntity
    {
        public string? Name { get; set; }

        public int? ParentDepartmentId { get; set; }

        public Department? ParentDepartment { get; set; }

        public List<Department> ChildDepartments { get; set; }

        public int? CostCodeId { get; set; }


    }
}
