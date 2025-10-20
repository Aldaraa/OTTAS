using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.BedFeature.GetAllBed
{
    public sealed class GetAllBedMapper : Profile
    {
        public GetAllBedMapper()
        {
            CreateMap<Bed, GetAllBedResponse>();
        }
    }

}

