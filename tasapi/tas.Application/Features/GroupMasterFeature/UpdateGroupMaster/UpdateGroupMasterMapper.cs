using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupMasterFeature.UpdateGroupMaster
{

    public sealed class UpdateGroupMasterMapper : Profile
    {
        public UpdateGroupMasterMapper()
        {
            CreateMap<UpdateGroupMasterRequest, GroupMaster>();
        }
    }
}
