using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeStatusFeature.DateLastEmployeeStatus
{
    public sealed record DateLastEmployeeStatusResponse
    {
        public int? DepartmentId { get; set; }

        public int? EmployerId { get; set; }

        public int? CostCodeId { get; set; }

        public int? ShiftId { get; set; }


    }
}
