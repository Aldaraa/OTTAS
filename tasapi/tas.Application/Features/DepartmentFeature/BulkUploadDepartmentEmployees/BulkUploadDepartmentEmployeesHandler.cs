using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.BulkUploadDepartmentEmployees
{
    public sealed class BulkUploadDepartmentEmployeesHandler : IRequestHandler<BulkUploadDepartmentEmployeesRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IMapper _mapper;

        public BulkUploadDepartmentEmployeesHandler(IUnitOfWork unitOfWork, IDepartmentRepository DepartmentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(BulkUploadDepartmentEmployeesRequest request, CancellationToken cancellationToken)
        {
            await _DepartmentRepository.BulkRequestEmployeesUpload(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
