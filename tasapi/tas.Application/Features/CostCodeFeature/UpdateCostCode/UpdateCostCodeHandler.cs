using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CostCodeFeature.CreateCostCode;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CostCodeFeature.UpdateCostCode
{
    public class UpdateCostCodeHandler : IRequestHandler<UpdateCostCodeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICostCodeRepository _costCodeRepository;
        private readonly IMapper _mapper;

        public UpdateCostCodeHandler(IUnitOfWork unitOfWork, ICostCodeRepository costCodeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _costCodeRepository = costCodeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateCostCodeRequest request, CancellationToken cancellationToken)
        {
            var costcode = _mapper.Map<CostCode>(request);
            await _costCodeRepository.CheckDuplicateData(costcode, c => c.Number);
            _costCodeRepository.Update(costcode);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
