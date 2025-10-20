using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CostCodeFeature.BulkUploadCostCode
{
    public sealed class BulkUploadCostCodeHandler : IRequestHandler<BulkUploadCostCodeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICostCodeRepository _CostCodeRepository;
        private readonly IMapper _mapper;

        public BulkUploadCostCodeHandler(IUnitOfWork unitOfWork, ICostCodeRepository costCodeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _CostCodeRepository = costCodeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(BulkUploadCostCodeRequest request, CancellationToken cancellationToken)
        {
            await _CostCodeRepository.BulkRequestUpload(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
