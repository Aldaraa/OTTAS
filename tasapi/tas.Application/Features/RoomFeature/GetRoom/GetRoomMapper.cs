using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.GetRoom
{
    public sealed class GetRoomMapper : Profile
    {
        public GetRoomMapper()
        {
            CreateMap<Room, GetRoomResponse>();
        }
    }

}

