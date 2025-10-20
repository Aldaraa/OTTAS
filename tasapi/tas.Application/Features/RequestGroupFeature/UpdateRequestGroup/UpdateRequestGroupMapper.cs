using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestGroupFeature.UpdateRequestGroup
{

    public sealed class UpdateRequestGroupMapper : Profile
    {
        public UpdateRequestGroupMapper()
        {
            CreateMap<UpdateRequestGroupRequest, RequestGroup>();
        }
    }
}
