using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RosterGroupFeature.UpdateRosterGroup;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterGroupFeature.DeleteRosterGroup
{

    public sealed class DeleteRosterGroupMapper : Profile
    {
        public DeleteRosterGroupMapper()
        {
            CreateMap<DeleteRosterGroupRequest, RosterGroup>();
        }
    }
}
