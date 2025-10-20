using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.PositioneFeature.BulkUploadPreviewPosition
{
    public sealed class BulkUploadPreviewPositionHandler : IRequestHandler<BulkRequestUploadPreviewPositionRequest, BulkRequestUploadPreviewPositionResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPositionRepository _PositionRepository;
        private readonly IMapper _mapper;

        public BulkUploadPreviewPositionHandler(IUnitOfWork unitOfWork, IPositionRepository PositionRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _PositionRepository = PositionRepository;
            _mapper = mapper;
        }

        public async Task<BulkRequestUploadPreviewPositionResponse>  Handle(BulkRequestUploadPreviewPositionRequest request, CancellationToken cancellationToken)
        {
            var returnData =  await _PositionRepository.BulkRequestUploadPreview(request, cancellationToken);
            return returnData;
        }
    }
}
