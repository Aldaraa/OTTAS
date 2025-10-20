using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.TransportModeFeature.UpdateTransportMode;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportModeFeature.DeleteTransportMode
{

    public sealed class DeleteTransportModeMapper : Profile
    {
        public DeleteTransportModeMapper()
        {
            CreateMap<DeleteTransportModeRequest, TransportMode>();
        }
    }
}
