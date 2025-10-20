using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.UpdateState;
using tas.Domain.Entities;

namespace tas.Application.Features.StateFeature.DeleteState
{

    public sealed class DeleteStateMapper : Profile
    {
        public DeleteStateMapper()
        {
            CreateMap<DeleteStateRequest, State>();
        }
    }
}
