using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportModeFeature.CreateTransportMode
{
    public sealed class CreateTransportModeMapper : Profile
    {
        public CreateTransportModeMapper()
        {
            CreateMap<CreateTransportModeRequest, TransportMode>();
        }
    }
}
