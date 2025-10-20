using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupDetailFeature.CreateGroupDetail
{
    public sealed class CreateGroupDetailMapper : Profile
    {
        public CreateGroupDetailMapper()
        {
            CreateMap<CreateGroupDetailRequest, GroupDetail>();
        }
    }
}
