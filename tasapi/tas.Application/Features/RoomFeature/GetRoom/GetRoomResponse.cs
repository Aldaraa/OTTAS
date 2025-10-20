
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.GetRoom
{ 
    public sealed record GetRoomResponse
    {
        public int Id { get; set; }
        public string Number { get; set; }

        public int? BedCount { get; set; }

        public int? Private { get; set; }

        public int? CampId { get; set; }

        public string? CampName { get; set; }

        public int? RoomTypeId { get; set; }

        public string? RoomTypeName { get; set; }

        public int? Active { get; set; }

        public int? VirtualRoom { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public List<Bed> BedList { get; set; }


    }
}