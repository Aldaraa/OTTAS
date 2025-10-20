using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.PositionFeature.BulkUploadPreviewPositionEmployees
{
    public sealed class BulkUploadPreviewPositionHandler : IRequestHandler<BulkUploadPreviewPositionEmployeesRequest, BulkUploadPreviewPositionEmployeesResponse>
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

        public async Task<BulkUploadPreviewPositionEmployeesResponse>  Handle(BulkUploadPreviewPositionEmployeesRequest request, CancellationToken cancellationToken)
        {
          var returnData =  await _PositionRepository.BulkRequestEmployeesPreview(request, cancellationToken);
            return returnData;
        }
    }
}
