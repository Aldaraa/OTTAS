using AutoMapper;
using MediatR;
using tas.Application.Features.CostCodeFeature.BulkUploadCostCodeEmployees;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CostCodeFeature.BulkUploadCostCode
{
    public sealed class BulkUploadCostCodeEmployeesHandler : IRequestHandler<BulkUploadCostCodeEmployeesRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICostCodeRepository _CostCodeRepository;
        private readonly IMapper _mapper;

        public BulkUploadCostCodeEmployeesHandler(IUnitOfWork unitOfWork, ICostCodeRepository costCodeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _CostCodeRepository = costCodeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(BulkUploadCostCodeEmployeesRequest request, CancellationToken cancellationToken)
        {
            await _CostCodeRepository.BulkRequestEmployeesUpload(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
