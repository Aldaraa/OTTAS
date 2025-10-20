using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.GetRequestNonSiteTravelOption
{
    public sealed class GetRequestNonSiteTravelOptionMapper : Profile
    {
        public GetRequestNonSiteTravelOptionMapper()
        {
            CreateMap<RequestNonSiteTravelOption, GetRequestNonSiteTravelOptionResponse>();
        }
    }

}

