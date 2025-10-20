using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeStatusFeature.VisualStatusGetEmployee
{
    public sealed record VisualStatusGetEmployeeResponse
    {
        public DateTime EventDate { get; set; }

        public int? ShiftId { get; set; }

        public string? ShiftCode { get; set; }

        public string? ShiftDescription { get; set; }

        public string? Color { get; set; }
        

    }
}
