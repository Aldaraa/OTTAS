using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestGroupFeature.CreateRequestGroup
{
    public sealed class CreateRequestGroupMapper : Profile
    {
        public CreateRequestGroupMapper()
        {
            CreateMap<CreateRequestGroupRequest, RequestGroup>();
        }
    }
}
