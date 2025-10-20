using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.DeleteEmployeeTransportBulk
{

    public sealed class DeleteEmployeeTransportBulkHandler : IRequestHandler<DeleteEmployeeTransportBulkRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public DeleteEmployeeTransportBulkHandler(IUnitOfWork unitOfWork, IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteEmployeeTransportBulkRequest request, CancellationToken cancellationToken)
        {
            await _EmployeeRepository.DeleteTransportBulk(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
