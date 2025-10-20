using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BusstopFeature.UpdateBusstop;
using tas.Domain.Entities;

namespace tas.Application.Features.BusstopFeature.DeleteBusstop
{

    public sealed class DeleteBusstopMapper : Profile
    {
        public DeleteBusstopMapper()
        {
            CreateMap<DeleteBusstopRequest, Busstop>();
        }
    }
}
