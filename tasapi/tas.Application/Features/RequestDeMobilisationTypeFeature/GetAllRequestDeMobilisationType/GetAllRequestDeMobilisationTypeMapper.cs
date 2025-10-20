using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDeMobilisationTypeFeature.GetAllRequestDeMobilisationType
{
    public sealed class GetAllRequestDeMobilisationTypeMapper : Profile
    {
        public GetAllRequestDeMobilisationTypeMapper()
        {
            CreateMap<RequestDeMobilisationType, GetAllRequestDeMobilisationTypeResponse>();
        }
    }

}

