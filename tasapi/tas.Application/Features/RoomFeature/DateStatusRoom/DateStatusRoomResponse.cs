
namespace tas.Application.Features.RoomFeature.DateStatusRoom
{ 
    public sealed record DateStatusRoomResponse
    {

        public List<DateStatusRoomBed> beds { get; set; }
    }


    public sealed record DateStatusRoomBed
    {
        public int Id { get; set; }

        public string? Description { get; set; }


        public DateStatusRoomBedGuest? Guest { get; set; }

    }

    public sealed record DateStatusRoomBedGuest
    {
        public int Id { get; set; }

        public string? Lastname { get; set; }

        public string? Firstname { get; set; }


    }
}