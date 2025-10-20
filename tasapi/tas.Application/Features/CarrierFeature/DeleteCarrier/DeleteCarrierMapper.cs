using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CarrierFeature.UpdateCarrier;
using tas.Domain.Entities;

namespace tas.Application.Features.CarrierFeature.DeleteCarrier
{

    public sealed class DeleteCarrierMapper : Profile
    {
        public DeleteCarrierMapper()
        {
            CreateMap<DeleteCarrierRequest, Carrier>();
        }
    }
}
