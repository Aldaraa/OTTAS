using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CostCodeFeature.CreateCostCode;
using tas.Domain.Entities;

namespace tas.Application.Features.CostCodeFeature.UpdateCostCode
{

    public sealed class UpdateCostCodeMapper : Profile
    {
        public UpdateCostCodeMapper()
        {
            CreateMap<UpdateCostCodeRequest, CostCode>();
        }
    }
}
