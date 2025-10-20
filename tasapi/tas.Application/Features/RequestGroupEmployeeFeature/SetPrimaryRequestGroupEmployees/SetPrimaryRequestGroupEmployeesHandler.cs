using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupEmployeeFeature.SetPrimaryRequestGroupEmployees
{

    public sealed class SetPrimaryRequestGroupEmployeesHandler : IRequestHandler<SetPrimaryRequestGroupEmployeesRequest, Unit>
    {
        private readonly IRequestGroupEmployeeRepository _RequestGroupEmployeeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SetPrimaryRequestGroupEmployeesHandler(IRequestGroupEmployeeRepository RequestGroupEmployeeRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _RequestGroupEmployeeRepository = RequestGroupEmployeeRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(SetPrimaryRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            await _RequestGroupEmployeeRepository.SetPrimaryGroupEmployees(request, cancellationToken);
            await   _unitOfWork.Save(cancellationToken);
            return Unit.Value;

        }
    }
}
