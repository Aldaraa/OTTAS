using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.FlightGroupMasterFeature.GetAllFlightGroupMaster
{
    public sealed class GetAllFlightGroupMasterMapper : Profile
    {
        public GetAllFlightGroupMasterMapper()
        {
            CreateMap<FlightGroupMaster, GetAllFlightGroupMasterResponse>();
        }
    }

}

