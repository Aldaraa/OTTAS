using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.ActiveTransportFeature.GetAllActiveTransport
{
    public sealed class GetAllActiveTransportMapper : Profile
    {
        public GetAllActiveTransportMapper()
        {
            CreateMap<ActiveTransport, GetAllActiveTransportResponse>();
        }
    }

}

