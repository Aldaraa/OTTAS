
namespace tas.Application.Features.PositionFeature.GetAllRoomType
{ 
    public sealed record GetAllRoomTypeResponse
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int Active { get; set; }

        public int? EmployeeCount { get; set; }

        public int? RoomCount { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}