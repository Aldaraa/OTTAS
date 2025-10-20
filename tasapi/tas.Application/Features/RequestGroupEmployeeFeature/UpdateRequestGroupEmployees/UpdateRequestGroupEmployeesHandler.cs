using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupEmployeeFeature.UpdateRequestGroupEmployees
{

    public sealed class UpdateRequestGroupEmployeesHandler : IRequestHandler<UpdateRequestGroupEmployeesRequest, Unit>
    {
        private readonly IRequestGroupEmployeeRepository _RequestGroupEmployeeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRequestGroupEmployeesHandler(IRequestGroupEmployeeRepository RequestGroupEmployeeRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _RequestGroupEmployeeRepository = RequestGroupEmployeeRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            await _RequestGroupEmployeeRepository.UpdateGroupEmployees(request, cancellationToken);
            await   _unitOfWork.Save(cancellationToken);
            return Unit.Value;

        }
    }
}
