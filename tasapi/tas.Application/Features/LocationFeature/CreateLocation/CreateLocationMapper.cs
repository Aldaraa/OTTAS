using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.LocationFeature.CreateLocation
{
    public sealed class CreateLocationMapper : Profile
    {
        public CreateLocationMapper()
        {
            CreateMap<CreateLocationRequest, Location>();
        }
    }
}
