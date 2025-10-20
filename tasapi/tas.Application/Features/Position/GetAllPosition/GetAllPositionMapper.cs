using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.PositionFeature.GetAllPosition
{
    public sealed class GetAllPositionMapper : Profile
    {
        public GetAllPositionMapper()
        {
            CreateMap<Position, GetAllPositionResponse>();
        }
    }

}

