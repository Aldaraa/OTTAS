using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeProfileStatusFeature.GetDateRangeProfileStatus
{
    public sealed record GetDateRangeProfileStatusResponse
    {
        public int Id { get; set; }

        public int? EmployeeId { get; set; }
        public DateTime? EventDate { get; set; }

        public string? CostCode { get; set; }


        public string? Department { get; set; }

        public string? Employer { get; set; }

        public string? Position { get; set; }

        public string? CampRoom { get; set; }

        public string? Location { get; set; }


    }
}
