using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.NationalityFeature.GetAllNationality
{
    public sealed class GetAllNationalityMapper : Profile
    {
        public GetAllNationalityMapper()
        {
            CreateMap<Nationality, GetAllNationalityResponse>();
        }
    }

}

