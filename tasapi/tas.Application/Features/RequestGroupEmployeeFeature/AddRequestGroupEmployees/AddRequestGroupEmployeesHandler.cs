using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupEmployeeFeature.AddRequestGroupEmployees
{

    public sealed class AddRequestGroupEmployeesHandler : IRequestHandler<AddRequestGroupEmployeesRequest, Unit>
    {
        private readonly IRequestGroupEmployeeRepository _RequestGroupEmployeeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AddRequestGroupEmployeesHandler(IRequestGroupEmployeeRepository RequestGroupEmployeeRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _RequestGroupEmployeeRepository = RequestGroupEmployeeRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AddRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            await _RequestGroupEmployeeRepository.AddGroupEmployees(request, cancellationToken);
            await   _unitOfWork.Save(cancellationToken);
            return Unit.Value;

        }
    }
}
