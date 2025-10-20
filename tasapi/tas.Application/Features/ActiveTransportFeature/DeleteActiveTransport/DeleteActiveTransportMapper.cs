using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ActiveTransportFeature.UpdateActiveTransport;
using tas.Domain.Entities;

namespace tas.Application.Features.ActiveTransportFeature.DeleteActiveTransport
{

    public sealed class DeleteActiveTransportMapper : Profile
    {
        public DeleteActiveTransportMapper()
        {
            CreateMap<DeleteActiveTransportRequest, ActiveTransport>();
        }
    }
}
