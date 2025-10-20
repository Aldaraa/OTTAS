using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.PeopleTypeFeature.GetAllPeopleType
{
    public sealed class GetAllPeopleTypeMapper : Profile
    {
        public GetAllPeopleTypeMapper()
        {
            CreateMap<PeopleType, GetAllPeopleTypeResponse>();
        }
    }

}

