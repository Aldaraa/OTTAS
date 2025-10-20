using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.PositionFeature.UpdatePosition;
using tas.Domain.Entities;

namespace tas.Application.Features.PositionFeature.DeletePosition
{

    public sealed class DeletePositionMapper : Profile
    {
        public DeletePositionMapper()
        {
            CreateMap<DeletePositionRequest, Position>();
        }
    }
}
