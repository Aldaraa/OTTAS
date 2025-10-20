using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestLocalHotelFeature.UpdateRequestLocalHotel;
using tas.Domain.Entities;

namespace tas.Application.Features.ColorFeature.UpdateColor
{

    public sealed class UpdateColorMapper : Profile
    {
        public UpdateColorMapper()
        {
            CreateMap<UpdateRequestLocalHotelRequest, RequestLocalHotel>();
        }
    }
}
