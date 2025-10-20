using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupDetailFeature.CreateGroupDetail;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupDetailFeature.UpdateGroupDetail
{

    public sealed class UpdateGroupDetailMapper : Profile
    {
        public UpdateGroupDetailMapper()
        {
            CreateMap<UpdateGroupDetailRequest, GroupDetail>();
        }
    }
}
