using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.BusstopFeature.UpdateBusstop
{

    public sealed class UpdateBusstopMapper : Profile
    {
        public UpdateBusstopMapper()
        {
            CreateMap<UpdateBusstopRequest, Busstop>();
        }
    }
}
