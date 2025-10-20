using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CostCodeFeature.UpdateCostCode;
using tas.Domain.Entities;

namespace tas.Application.Features.CostCodeFeature.DeleteCostCode
{

    public sealed class DeleteCostCodeMapper : Profile
    {
        public DeleteCostCodeMapper()
        {
            CreateMap<DeleteCostCodeRequest, CostCode>();
        }
    }
}
