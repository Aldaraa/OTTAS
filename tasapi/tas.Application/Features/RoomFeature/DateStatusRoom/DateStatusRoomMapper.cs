using AutoMapper;
using tas.Application.Features.RoomFeature.DateStatusRoom;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.DateStatusRoom
{
    public sealed class DateStatusRoomMapper : Profile
    {
        public DateStatusRoomMapper()
        {
            CreateMap<Room, DateStatusRoomResponse>();
        }
    }

}

