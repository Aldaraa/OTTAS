using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.BedFeature.GetBed
{
    public sealed class GetBedMapper : Profile
    {
        public GetBedMapper()
        {
            CreateMap<Bed, GetBedResponse>();
        }
    }

}

