using AutoMapper;
using tas.Application.Features.PositionFeature.GetAllRoomType;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomTypeFeature.GetAllRoomType
{
    public sealed class GetAllRoomTypeMapper : Profile
    {
        public GetAllRoomTypeMapper()
        {
            CreateMap<RoomType, GetAllRoomTypeResponse>();
        }
    }

}

