using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestNonSiteTicketConfigFeature.GetAllRequestNonSiteTicketConfig
{
    public sealed class GetAllRequestNonSiteTicketConfigMapper : Profile
    {
        public GetAllRequestNonSiteTicketConfigMapper()
        {
            CreateMap<RequestNonSiteTicketConfig, GetAllRequestNonSiteTicketConfigResponse>();
        }
    }

}

