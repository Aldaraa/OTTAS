using AutoMapper;
using tas.Application.Features.RoomFeature.FindAvailableRoom;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.FindAvailableRoom
{
    public sealed class FindAvailableRoomMapper : Profile
    {
        public FindAvailableRoomMapper()
        {
            CreateMap<Room, FindAvailableRoomResponse>();
        }
    }

}

