
using tas.Domain.Entities;

namespace tas.Application.Features.RosterFeature.GetRoster
{ 
    public sealed record GetRosterResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public int RosterGroupId { get; set; }

        public string RosterGroupName { get; set; }


        public int Active { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}