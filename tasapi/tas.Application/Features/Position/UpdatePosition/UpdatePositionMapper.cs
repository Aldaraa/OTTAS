using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.PositionFeature.UpdatePosition
{

    public sealed class UpdatePositionMapper : Profile
    {
        public UpdatePositionMapper()
        {
            CreateMap<UpdatePositionRequest, Position>();
        }
    }
}
