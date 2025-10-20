using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RosterFeature.UpdateRoster;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterFeature.DeleteRoster
{

    public sealed class DeleteRosterMapper : Profile
    {
        public DeleteRosterMapper()
        {
            CreateMap<DeleteRosterRequest, Roster>();
        }
    }
}
