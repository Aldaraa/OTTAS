using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmenteFeature.BulkUploadPreviewDepartmentEmployees
{
    public sealed class BulkUploadPreviewDepartmentHandler : IRequestHandler<BulkUploadPreviewDepartmentEmployeesRequest, BulkUploadPreviewDepartmentEmployeesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IMapper _mapper;

        public BulkUploadPreviewDepartmentHandler(IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _DepartmentRepository = departmentRepository;
            _mapper = mapper;
        }

        public async Task<BulkUploadPreviewDepartmentEmployeesResponse>  Handle(BulkUploadPreviewDepartmentEmployeesRequest request, CancellationToken cancellationToken)
        {
          var returnData =  await _DepartmentRepository.BulkRequestEmployeesPreview(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return returnData;
        }
    }
}
