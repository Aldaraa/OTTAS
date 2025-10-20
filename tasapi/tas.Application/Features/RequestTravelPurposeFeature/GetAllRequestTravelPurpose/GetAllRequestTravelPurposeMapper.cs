using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestTravelPurposeFeature.GetAllRequestTravelPurpose
{
    public sealed class GetAllRequestTravelPurposeMapper : Profile
    {
        public GetAllRequestTravelPurposeMapper()
        {
            CreateMap<RequestTravelPurpose, GetAllRequestTravelPurposeResponse>();
        }
    }

}

