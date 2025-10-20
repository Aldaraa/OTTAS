using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.StateFeature.CreateState
{
    public sealed class CreateStateMapper : Profile
    {
        public CreateStateMapper()
        {
            CreateMap<CreateStateRequest, State>();
        }
    }
}
