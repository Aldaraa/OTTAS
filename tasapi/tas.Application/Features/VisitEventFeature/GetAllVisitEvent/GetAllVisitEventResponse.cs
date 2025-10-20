using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.GetAllVisitEvent
{
    public sealed record GetAllVisitEventResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Requester { get; set; }


        public int Active { get; set; }

        public int? HeadCount { get; set; }

        public string? Status { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string? InDescr { get; set; }

        public int? InScheduleId { get; set; }

        public string? OutDescr { get; set; }

        public int? OutScheduleId { get; set; }


    }
}
