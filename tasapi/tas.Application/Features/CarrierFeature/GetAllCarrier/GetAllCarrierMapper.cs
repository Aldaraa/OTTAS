using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.CarrierFeature.GetAllCarrier
{
    public sealed class GetAllCarrierMapper : Profile
    {
        public GetAllCarrierMapper()
        {
            CreateMap<Carrier, GetAllCarrierResponse>();
        }
    }

}

