using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.GetScheduleDetailTransport
{
 
    public sealed record GetScheduleDetailTransportResponse
    {
        public int Id { get; set; }
        public string? FullName { get; set; }

        public string? Department { get; set; }

        public string? CostrCodeDescr { get; set; }

        public string? Employer { get; set; }

        public string? Description { get; set; }

        public  DateTime? DateCreated { get; set; }
        public  DateTime? DateUpdated { get; set; }

        public int? EmployeeId { get; set; }

        public string? Status { get; set; }

    }
}
