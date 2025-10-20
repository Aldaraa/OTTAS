
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.GetVirtualRoom
{ 
    public sealed record GetVirtualRoomResponse
    {
        public int Id { get; set; }

        public int? NoAccommdationId { get; set; }

        public int? KhanbogdRoomId { get; set; }


    

    }
}