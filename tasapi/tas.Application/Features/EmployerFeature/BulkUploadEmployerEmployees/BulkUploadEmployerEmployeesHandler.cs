using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployerFeature.BulkUploadEmployerEmployees
{
    public sealed class BulkUploadEmployerEmployeesHandler : IRequestHandler<BulkUploadEmployerEmployeesRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployerRepository _EmployerRepository;
        private readonly IMapper _mapper;

        public BulkUploadEmployerEmployeesHandler(IUnitOfWork unitOfWork, IEmployerRepository EmployerRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployerRepository = EmployerRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(BulkUploadEmployerEmployeesRequest request, CancellationToken cancellationToken)
        {
            await _EmployerRepository.BulkRequestEmployeesUpload(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
