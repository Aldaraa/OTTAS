using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.LocationFeature.UpdateLocation;
using tas.Domain.Entities;

namespace tas.Application.Features.LocationFeature.DeleteLocation
{

    public sealed class DeleteLocationMapper : Profile
    {
        public DeleteLocationMapper()
        {
            CreateMap<DeleteLocationRequest, Location>();
        }
    }
}
