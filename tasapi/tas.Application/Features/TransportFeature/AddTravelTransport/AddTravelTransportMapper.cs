using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportFeature.AddTravelTransport
{
    public sealed class AddTravelTransportMapper : Profile
    {
        public AddTravelTransportMapper()
        {
            CreateMap<AddTravelTransportRequest, Transport>();
        }
    }
}
