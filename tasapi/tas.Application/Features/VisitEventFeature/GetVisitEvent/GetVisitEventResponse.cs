using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.GetVisitEvent
{
    public sealed record GetVisitEventResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public int Active { get; set; }

        public int? HeadCount { get; set; }

        public string? Status { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? InScheduleId { get; set; }

        public int? OutScheduleId { get; set; }


        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public List<GetVisitEventResponseEmployees> Employees { get; set; }


    }

    public sealed record GetVisitEventResponseEmployees
    { 
        public int Id { get; set; }

        public int? Active { get; set; }
        public int EmployeeId { get; set; }

        public string? Lastname { get; set; }

        public string? Firstname { get; set; }

        public int? InScheduleId { get; set; }

        public int? OutScheduleId { get; set; }

        public DateTime? InEventDate { get; set; }

        public DateTime? OutEventDate { get; set; }

        public string? InStatus { get; set; }

        public string? OutStatus { get; set;  }

        public string? InDescr { get; set; }

        public string? OutDescr { get; set; }


        public string? Direction { get; set; }



    }
}
