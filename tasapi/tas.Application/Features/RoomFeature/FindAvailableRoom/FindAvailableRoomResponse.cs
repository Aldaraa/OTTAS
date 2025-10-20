
namespace tas.Application.Features.RoomFeature.FindAvailableRoom
{ 
    public sealed record FindAvailableRoomResponse
    {

        public string? roomNumber { get; set; }
        public int? RoomId { get; set; }

        public int? BedCount { get; set; }



        public int? VirtulRoom { get; set; }
    }
}