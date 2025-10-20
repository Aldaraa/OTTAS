using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestGroupFeature.UpdateRequestGroup;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestGroupFeature.DeleteRequestGroup
{

    public sealed class DeleteRequestGroupMapper : Profile
    {
        public DeleteRequestGroupMapper()
        {
            CreateMap<DeleteRequestGroupRequest, RequestGroup>();
        }
    }
}
