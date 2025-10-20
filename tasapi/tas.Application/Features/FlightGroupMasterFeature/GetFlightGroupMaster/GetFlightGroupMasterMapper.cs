using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.FlightGroupMasterFeature.GetFlightGroupMaster
{
    public sealed class GetFlightGroupMasterMapper : Profile
    {
        public GetFlightGroupMasterMapper()
        {
            CreateMap<FlightGroupMaster, GetFlightGroupMasterResponse>();
        }
    }

}

