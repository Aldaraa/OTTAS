using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestAirportFeature.GetAllRequestAirport
{
    public sealed class GetAllRequestAirportMapper : Profile
    {
        public GetAllRequestAirportMapper()
        {
            CreateMap<RequestAirport, GetAllRequestAirportResponse>();
        }
    }

}

