using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.DeleteEmployee
{

    public sealed class DeleteEmployeeHandler : IRequestHandler<DeleteEmployeeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public DeleteEmployeeHandler(IUnitOfWork unitOfWork, IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteEmployeeRequest request, CancellationToken cancellationToken)
        {
            var Employee = _mapper.Map<Employee>(request);
            _EmployeeRepository.Delete(Employee);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
