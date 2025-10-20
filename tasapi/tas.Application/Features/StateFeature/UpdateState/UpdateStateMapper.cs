using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.StateFeature.UpdateState
{

    public sealed class UpdateStateMapper : Profile
    {
        public UpdateStateMapper()
        {
            CreateMap<UpdateStateRequest, State>();
        }
    }
}
