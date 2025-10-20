using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDelegateFeature.CreateRequestDelegate
{
    public sealed class CreateRequestDelegateMapper : Profile
    {
        public CreateRequestDelegateMapper()
        {
            CreateMap<CreateRequestDelegateRequest, RequestDelegates>();
        }
    }
}
