using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestLocalHotelFeature.UpdateRequestLocalHotel;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestLocalHotelFeature.DeleteRequestLocalHotel
{

    public sealed class DeleteRequestLocalHotelMapper : Profile
    {
        public DeleteRequestLocalHotelMapper()
        {
            CreateMap<DeleteRequestLocalHotelRequest, RequestLocalHotel>();
        }
    }
}
