using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestTravelAgentFeature.GetAllRequestTravelAgent
{
    public sealed class GetAllRequestTravelAgentMapper : Profile
    {
        public GetAllRequestTravelAgentMapper()
        {
            CreateMap<RequestTravelAgent, GetAllRequestTravelAgentResponse>();
        }
    }

}

