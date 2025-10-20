using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.CostCodeFeature.GetAllCostCode
{

    public sealed class GetAllCostCodeHandler : IRequestHandler<GetAllCostCodeRequest, List<GetAllCostCodeResponse>>
    {
        private readonly ICostCodeRepository _costCodeRepository;
        private readonly IMapper _mapper;

        public GetAllCostCodeHandler(ICostCodeRepository costcodeRepository, IMapper mapper)
        {
            _costCodeRepository = costcodeRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllCostCodeResponse>> Handle(GetAllCostCodeRequest request, CancellationToken cancellationToken)
        {
            var data = await _costCodeRepository.GetAllData(request, cancellationToken);
            return _mapper.Map<List<GetAllCostCodeResponse>>(data);

        }
    }
}
