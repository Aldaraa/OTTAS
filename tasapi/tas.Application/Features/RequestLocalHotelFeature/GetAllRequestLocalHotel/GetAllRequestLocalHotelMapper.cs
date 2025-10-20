using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestLocalHotelFeature.GetAllRequestLocalHotel
{
    public sealed class GetAllRequestLocalHotelMapper : Profile
    {
        public GetAllRequestLocalHotelMapper()
        {
            CreateMap<RequestLocalHotel, GetAllRequestLocalHotelResponse>();
        }
    }

}

