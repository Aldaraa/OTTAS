using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CostCodeFeature.UpdateCostCode;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CostCodeFeature.DeleteCostCode
{

    public sealed class DeleteCostCodeHandler : IRequestHandler<DeleteCostCodeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICostCodeRepository _costCodeRepository;
        private readonly IMapper _mapper;

        public DeleteCostCodeHandler(IUnitOfWork unitOfWork, ICostCodeRepository costCodeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _costCodeRepository = costCodeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteCostCodeRequest request, CancellationToken cancellationToken)
        {
            var costcode = _mapper.Map<CostCode>(request);
            _costCodeRepository.Delete(costcode);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
