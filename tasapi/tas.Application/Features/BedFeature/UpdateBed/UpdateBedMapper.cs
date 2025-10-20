using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.BedFeature.UpdateBed
{

    public sealed class UpdateBedMapper : Profile
    {
        public UpdateBedMapper()
        {
            CreateMap<UpdateBedRequest, Bed>();
        }
    }
}
