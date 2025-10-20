using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterGroupFeature.UpdateRosterGroup
{

    public sealed class UpdateRosterGroupMapper : Profile
    {
        public UpdateRosterGroupMapper()
        {
            CreateMap<UpdateRosterGroupRequest, RosterGroup>();
        }
    }
}
