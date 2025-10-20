using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.BulkUploadPreviewEmployee
{
    public sealed class BulkUploadPreviewEmployeeHandler : IRequestHandler<BulkUploadPreviewEmployeeRequest, BulkUploadPreviewEmployeeResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public BulkUploadPreviewEmployeeHandler(IUnitOfWork unitOfWork, IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<BulkUploadPreviewEmployeeResponse>  Handle(BulkUploadPreviewEmployeeRequest request, CancellationToken cancellationToken)
        {
          var returnData =  await _EmployeeRepository.BulkRequestUploadPreview(request, cancellationToken);
            return returnData;
        }
    }
}
