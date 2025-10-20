using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterGroupFeature.CreateRosterGroup
{
    public sealed class CreateRosterGroupMapper : Profile
    {
        public CreateRosterGroupMapper()
        {
            CreateMap<CreateRosterGroupRequest, RosterGroup>();
        }
    }
}
