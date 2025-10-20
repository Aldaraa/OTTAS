using AutoMapper;
using tas.Application.Features.RoomFeature.GetAllRoom;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.GetAllRoom
{
    public sealed class GetAllRoomMapper : Profile
    {
        public GetAllRoomMapper()
        {
            CreateMap<Room, GetAllRoomResponse>();
        }
    }

}

