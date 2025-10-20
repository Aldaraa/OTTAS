using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCodeEmployees
{
    public sealed class BulkUploadPreviewCostCodeHandler : IRequestHandler<BulkUploadPreviewCostCodeEmployeesRequest, BulkUploadPreviewCostCodeEmployeesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICostCodeRepository _CostCodeRepository;
        private readonly IMapper _mapper;

        public BulkUploadPreviewCostCodeHandler(IUnitOfWork unitOfWork, ICostCodeRepository costCodeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _CostCodeRepository = costCodeRepository;
            _mapper = mapper;
        }

        public async Task<BulkUploadPreviewCostCodeEmployeesResponse>  Handle(BulkUploadPreviewCostCodeEmployeesRequest request, CancellationToken cancellationToken)
        {
          var returnData =  await _CostCodeRepository.BulkRequestEmployeesPreview(request, cancellationToken);
            return returnData;
        }
    }
}
