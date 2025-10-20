using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.CarrierFeature.CreateCarrier
{
    public sealed class CreateCarrierMapper : Profile
    {
        public CreateCarrierMapper()
        {
            CreateMap<CreateCarrierRequest, Carrier>();
        }
    }
}
