using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.CostCodeFeature.GetAllCostCode
{
    public sealed record GetAllCostCodeResponse
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Number { get; set; }

        public string? Description { get; set; }
        public int Active { get; set; }

        public int? EmployeeCount { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
