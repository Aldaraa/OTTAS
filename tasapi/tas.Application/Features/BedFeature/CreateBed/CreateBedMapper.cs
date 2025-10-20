using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.BedFeature.CreateBed
{
    public sealed class CreateBedMapper : Profile
    {
        public CreateBedMapper()
        {
            CreateMap<CreateBedRequest, Bed>();
        }
    }
}
