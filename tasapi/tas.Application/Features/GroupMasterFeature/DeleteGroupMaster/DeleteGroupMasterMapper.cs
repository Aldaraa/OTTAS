using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupMasterFeature.UpdateGroupMaster;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupMasterFeature.DeleteGroupMaster
{

    public sealed class DeleteGroupMasterMapper : Profile
    {
        public DeleteGroupMasterMapper()
        {
            CreateMap<DeleteGroupMasterRequest, GroupMaster>();
        }
    }
}
