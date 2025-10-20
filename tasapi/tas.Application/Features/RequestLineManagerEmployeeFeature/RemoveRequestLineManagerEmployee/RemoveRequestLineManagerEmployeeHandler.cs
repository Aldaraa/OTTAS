using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestLineManagerEmployeeFeature.RemoveRequestLineManagerEmployee
{
    public sealed class RemoveRequestLineManagerEmployeeHandler : IRequestHandler<RemoveRequestLineManagerEmployeeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestLineManagerEmployeeRepository _IRequestLineManagerEmployeeRepository;

        public RemoveRequestLineManagerEmployeeHandler(IUnitOfWork unitOfWork, IRequestLineManagerEmployeeRepository RequestGroupRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _IRequestLineManagerEmployeeRepository = RequestGroupRepository;
        }

        public async Task<Unit>  Handle(RemoveRequestLineManagerEmployeeRequest request, CancellationToken cancellationToken)
        {
            await _IRequestLineManagerEmployeeRepository.RemoveRequestLineManagerEmployee(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
