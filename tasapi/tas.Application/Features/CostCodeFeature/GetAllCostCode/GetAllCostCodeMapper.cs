using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.CostCodeFeature.GetAllCostCode
{
    public sealed class GetAllCostCodeMapper : Profile
    {
        public GetAllCostCodeMapper()
        {
            CreateMap<CostCode, GetAllCostCodeResponse>();
        }
    }

}

