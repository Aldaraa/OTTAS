using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.CostCodeFeature.CreateCostCode
{
    public sealed class CreateCostCodeMapper : Profile
    {
        public CreateCostCodeMapper()
        {
            CreateMap<CreateCostCodeRequest, CostCode>();
        }
    }
}
