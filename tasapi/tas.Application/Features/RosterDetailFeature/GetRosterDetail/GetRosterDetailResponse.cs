using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterDetailFeature.GetRosterDetail
{
    public sealed record GetRosterDetailResponse
    {
        public int Id { get; set; }

        public int? ShiftId { get; set; }

        public string? ShiftColor { get; set; }

        public string? ShiftCode { get; set; }

        public string? ShiftName { get; set; }

        public int? SeqNumber { get; set; }

        public int? RosterId { get; set; }

        public int? DaysOn { get; set; }

        public int? OnSite { get; set; }


        public string? RosterName { get; set; }

        public int Active { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
