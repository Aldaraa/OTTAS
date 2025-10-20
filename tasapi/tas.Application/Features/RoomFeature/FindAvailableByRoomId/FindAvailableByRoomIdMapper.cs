using AutoMapper;
using tas.Application.Features.RoomFeature.FindAvailableRoom;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.FindAvailableByRoomId
{
    public sealed class FindAvailableByRoomIdMapper : Profile
    {
        public FindAvailableByRoomIdMapper()
        {
            CreateMap<Room, FindAvailableByRoomIdResponse>();
        }
    }

}

