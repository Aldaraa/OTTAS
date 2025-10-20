using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.PositionFeature.AllPosition
{
    public sealed class AllPositionMapper : Profile
    {
        public AllPositionMapper()
        {
            CreateMap<Position, AllPositionResponse>();
        }
    }

}

