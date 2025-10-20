using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.CampFeature.GetAllCamp
{
    public sealed class GetAllCampMapper : Profile
    {
        public GetAllCampMapper()
        {
            CreateMap<Camp, GetAllCampResponse>();
        }
    }

}

