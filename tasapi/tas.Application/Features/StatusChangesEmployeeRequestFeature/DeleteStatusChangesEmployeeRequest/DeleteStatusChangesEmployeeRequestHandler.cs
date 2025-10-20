using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.StatusChangesEmployeeRequestFeature.DeleteStatusChangesEmployeeRequest
{

    public sealed class DeleteStatusChangesEmployeeRequestHandler : IRequestHandler<DeleteStatusChangesEmployeeRequestRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStatusChangesEmployeeRequestRepository _StatusChangesEmployeeRequestRepository;
        private readonly IMapper _mapper;

        public DeleteStatusChangesEmployeeRequestHandler(IUnitOfWork unitOfWork, IStatusChangesEmployeeRequestRepository StatusChangesEmployeeRequestRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _StatusChangesEmployeeRequestRepository = StatusChangesEmployeeRequestRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteStatusChangesEmployeeRequestRequest request, CancellationToken cancellationToken)
        {
            await _StatusChangesEmployeeRequestRepository.DeleteStatusChangesEmployeeRequest(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
