using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.BulkUploadDepartment
{
    public sealed class BulkUploadDepartmentHandler : IRequestHandler<BulkUploadDepartmentRequest, BulkUploadDepartmentResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IMapper _mapper;

        public BulkUploadDepartmentHandler(IUnitOfWork unitOfWork, IDepartmentRepository DepartmentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
        }

        public async Task<BulkUploadDepartmentResponse>  Handle(BulkUploadDepartmentRequest request, CancellationToken cancellationToken)
        {
          var returnData =  await _DepartmentRepository.BulkRequestUpload(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return returnData;
        }
    }
}
