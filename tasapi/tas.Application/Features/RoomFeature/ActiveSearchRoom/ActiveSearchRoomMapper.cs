using AutoMapper;
using tas.Application.Features.RoomFeature.ActiveSearchRoom;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.ActiveSearchRoom
{
    public sealed class ActiveSearchRoomMapper : Profile
    {
        public ActiveSearchRoomMapper()
        {
            CreateMap<Room, ActiveSearchRoomResponse>();
        }
    }

}

