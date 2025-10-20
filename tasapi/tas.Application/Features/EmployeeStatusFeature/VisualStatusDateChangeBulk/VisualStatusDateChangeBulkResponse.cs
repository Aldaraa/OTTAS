using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeStatusFeature.VisualStatusDateChangeBulk
{
    public sealed record VisualStatusDateChangeBulkResponse
    {
        public int Id { get; set; }
        public string? FullName { get; set; }

        public VisualStatusDateChangeBulkResponseReson? SkippedReason { get; set; }

    }

    public sealed record VisualStatusDateChangeBulkResponseReson
    { 
        public string? Name { get; set; }

        public List<string> Days { get; set; }
    }
}
