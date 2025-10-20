using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupDetailFeature.UpdateGroupDetail;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupDetailFeature.DeleteGroupDetail
{

    public sealed class DeleteGroupDetailMapper : Profile
    {
        public DeleteGroupDetailMapper()
        {
            CreateMap<DeleteGroupDetailRequest, GroupDetail>();
        }
    }
}
