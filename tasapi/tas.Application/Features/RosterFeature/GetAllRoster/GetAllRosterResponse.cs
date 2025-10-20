
namespace tas.Application.Features.RosterFeature.GetAllRoster
{ 
    public sealed record GetAllRosterResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public int RosterGroupId { get; set; }

        public string? RosterGroupName { get; set; }

        public int DetailCount { get; set; }

        public int Active { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public int? EmployeeCount { get; set;}
    }
}