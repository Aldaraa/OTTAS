using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCode
{
    public sealed class BulkUploadPreviewCostCodeHandler : IRequestHandler<BulkUploadPreviewCostCodeRequest, BulkUploadPreviewCostCodeResponse>
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

        public async Task<BulkUploadPreviewCostCodeResponse>  Handle(BulkUploadPreviewCostCodeRequest request, CancellationToken cancellationToken)
        {
          var returnData =  await _CostCodeRepository.BulkRequestPreview(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return returnData;
        }
    }
}
