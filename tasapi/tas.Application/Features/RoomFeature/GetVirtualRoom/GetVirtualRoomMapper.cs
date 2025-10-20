using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.GetVirtualRoom
{
    public sealed class GetVirtualRoomMapper : Profile
    {
        public GetVirtualRoomMapper()
        {
            CreateMap<Room, GetVirtualRoomResponse>();
        }
    }

}

