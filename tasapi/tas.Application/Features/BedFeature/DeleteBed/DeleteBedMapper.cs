using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.UpdateBed;
using tas.Domain.Entities;

namespace tas.Application.Features.BedFeature.DeleteBed
{

    public sealed class DeleteBedMapper : Profile
    {
        public DeleteBedMapper()
        {
            CreateMap<DeleteBedRequest, Bed>();
        }
    }
}
