using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportModeFeature.UpdateTransportMode
{

    public sealed class UpdateTransportModeMapper : Profile
    {
        public UpdateTransportModeMapper()
        {
            CreateMap<UpdateTransportModeRequest, TransportMode>();
        }
    }
}
