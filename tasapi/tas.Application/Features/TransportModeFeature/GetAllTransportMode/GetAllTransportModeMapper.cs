using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportModeFeature.GetAllTransportMode
{
    public sealed class GetAllTransportModeMapper : Profile
    {
        public GetAllTransportModeMapper()
        {
            CreateMap<TransportMode, GetAllTransportModeResponse>();
        }
    }

}

