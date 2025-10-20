using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.BulkUploadEmployeeGroup
{
    public sealed class BulkUploadEmployeeGroupHandler : IRequestHandler<BulkUploadEmployeeGroupRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public BulkUploadEmployeeGroupHandler(IUnitOfWork unitOfWork, IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(BulkUploadEmployeeGroupRequest request, CancellationToken cancellationToken)
        {
            await _EmployeeRepository.BulkRequestGroupUpload(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
