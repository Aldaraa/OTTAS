using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.PositionFeature.GetAllPosition;
using tas.Domain.Entities;

namespace tas.Application.Features.LocationFeature.GetAllLocation
{

    public sealed class GetAllLocationMapper : Profile
    {
        public GetAllLocationMapper()
        {
            CreateMap<Location, GetAllLocationResponse>();
        }
    }

}
