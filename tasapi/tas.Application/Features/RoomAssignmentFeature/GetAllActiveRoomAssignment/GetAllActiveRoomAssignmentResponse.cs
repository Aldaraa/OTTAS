
namespace tas.Application.Features.RoomFeature.GetAllActiveRoomAssignment
{ 
    public sealed record GetAllActiveRoomAssignmentResponse
    {
        public int Id { get; set; }
        public string Number { get; set; }

        public int ?BedCount { get; set; }

        public int? Private { get; set; }

        public int? CampId { get; set; }

        public string? CampName { get; set; }

        public int? RoomTypeId { get; set; }

        public string? RoomTypeName { get; set; }

        public int? EmployeeCount { get; set; }

        public int? VirtualRoom { get; set; }
        public int? Active { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}