using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestLineManagerEmployeeFeature.CreateRequestLineManagerEmployee
{
    public sealed class CreateRequestLineManagerEmployeeHandler : IRequestHandler<CreateRequestLineManagerEmployeeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestLineManagerEmployeeRepository _IRequestLineManagerEmployeeRepository;

        public CreateRequestLineManagerEmployeeHandler(IUnitOfWork unitOfWork, IRequestLineManagerEmployeeRepository RequestGroupRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _IRequestLineManagerEmployeeRepository = RequestGroupRepository;
        }

        public async Task<Unit>  Handle(CreateRequestLineManagerEmployeeRequest request, CancellationToken cancellationToken)
        {
            await _IRequestLineManagerEmployeeRepository.CreateRequestLineManagerEmployee(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
