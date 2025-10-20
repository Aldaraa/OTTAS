using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupEmployeeFeature.RemoveRequestGroupEmployees
{

    public sealed class RemoveRequestGroupEmployeesHandler : IRequestHandler<RemoveRequestGroupEmployeesRequest, Unit>
    {
        private readonly IRequestGroupEmployeeRepository _RequestGroupEmployeeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveRequestGroupEmployeesHandler(IRequestGroupEmployeeRepository RequestGroupEmployeeRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _RequestGroupEmployeeRepository = RequestGroupEmployeeRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(RemoveRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            await _RequestGroupEmployeeRepository.RemoveGroupEmployees(request, cancellationToken);
            await   _unitOfWork.Save(cancellationToken);
            return Unit.Value;

        }
    }
}
