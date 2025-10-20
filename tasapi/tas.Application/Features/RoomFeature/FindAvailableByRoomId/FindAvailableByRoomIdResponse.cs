
namespace tas.Application.Features.RoomFeature.FindAvailableByRoomId
{ 
    public sealed record FindAvailableByRoomIdResponse
    {
        public bool status { get; set; }

        public int? RoomId { get; set; }
        public List<FindAvailableByRoomIdRoomData> RoomData { get; set; }
    }

    public sealed record FindAvailableByRoomIdRoomData
    {
        public int RoomId { get; set; }
        public DateTime EventDate { get; set; }

        public string? roomNumber { get; set; }

        public int? BedCount { get; set; }

        public int? ActiveBeds { get; set; }


    }
}