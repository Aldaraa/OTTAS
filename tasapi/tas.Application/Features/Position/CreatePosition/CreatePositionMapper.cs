using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.PositionFeature.CreatePosition
{
    public sealed class CreatePositionMapper : Profile
    {
        public CreatePositionMapper()
        {
            CreateMap<CreatePositionRequest, Position>();
        }
    }
}
