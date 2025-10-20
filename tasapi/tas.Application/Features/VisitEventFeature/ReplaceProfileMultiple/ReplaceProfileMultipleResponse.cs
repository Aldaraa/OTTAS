using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.ReplaceProfileMultiple
{
    public sealed record ReplaceProfileMultipleResponse
    {
        public int EmployeeId { get; set; }
        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Reason { get; set; }

    }
}
