using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.BulkUploadPreviewEmployeeGroup
{
    public sealed class BulkUploadPreviewEmployeeGroupHandler : IRequestHandler<BulkUploadPreviewEmployeeGroupRequest, BulkUploadPreviewEmployeeGroupResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public BulkUploadPreviewEmployeeGroupHandler(IUnitOfWork unitOfWork, IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<BulkUploadPreviewEmployeeGroupResponse>  Handle(BulkUploadPreviewEmployeeGroupRequest request, CancellationToken cancellationToken)
        {
          var returnData =  await _EmployeeRepository.BulkRequestUploadGroupPreview(request, cancellationToken);
            return returnData;
        }
    }
}
