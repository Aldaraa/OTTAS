using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupMembersFeature.CreateGroupMembers
{
    public sealed class CreateGroupMembersMapper : Profile
    {
        public CreateGroupMembersMapper()
        {
            CreateMap<CreateGroupMembersRequest, GroupMaster>();
        }
    }
}
