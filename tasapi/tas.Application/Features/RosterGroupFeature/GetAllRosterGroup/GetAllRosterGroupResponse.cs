using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterGroupFeature.GetAllRosterGroup
{
    public sealed record GetAllRosterGroupResponse
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int Active { get; set; }

        public int? DetailCount { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public int? EmployeeCount { get; set; }
    }
}
