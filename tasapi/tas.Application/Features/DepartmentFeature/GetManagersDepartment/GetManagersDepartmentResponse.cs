using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.GetManagersDepartment
{
    public sealed record GetManagersDepartmentResponse
    {
        public int Id { get; set; }

        public int? DepartmentId { get; set; }
        public string? Name { get; set; }

        public DateTime? DateCreated { get; set; }
    }

   
}
