using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.ProfileFieldFeature.GetAllProfileField
{
    public sealed class GetAllProfileFieldMapper : Profile
    {
        public GetAllProfileFieldMapper()
        {
            CreateMap<ProfileField, GetAllProfileFieldResponse>();
        }
    }

}

