using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.BusstopFeature.GetAllBusstop
{
    public sealed class GetAllBusstopMapper : Profile
    {
        public GetAllBusstopMapper()
        {
            CreateMap<Busstop, GetAllBusstopResponse>();
        }
    }

}

