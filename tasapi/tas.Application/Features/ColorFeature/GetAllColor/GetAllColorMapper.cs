using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.ColorFeature.GetAllColor
{
    public sealed class GetAllColorMapper : Profile
    {
        public GetAllColorMapper()
        {
            CreateMap<Color, GetAllColorResponse>();
        }
    }

}

