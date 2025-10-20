using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupEmployeeFeature.OrderRequestGroupEmployees
{

    public sealed class OrderRequestGroupEmployeesHandler : IRequestHandler<OrderRequestGroupEmployeesRequest, Unit>
    {
        private readonly IRequestGroupEmployeeRepository _RequestGroupEmployeeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderRequestGroupEmployeesHandler(IRequestGroupEmployeeRepository RequestGroupEmployeeRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _RequestGroupEmployeeRepository = RequestGroupEmployeeRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(OrderRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            await _RequestGroupEmployeeRepository.OrderGroupEmployees(request, cancellationToken);
            await   _unitOfWork.Save(cancellationToken);
            return Unit.Value;

        }
    }
}
