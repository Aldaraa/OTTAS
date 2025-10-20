using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.RemoveRoomAssignmentOwnership
{
    public sealed class RemoveRoomAssignmentOwnershipHandler : IRequestHandler<RemoveRoomAssignmentOwnershipRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomAssignmentRepository _RoomAssignmentRepository;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;

        public RemoveRoomAssignmentOwnershipHandler(IUnitOfWork unitOfWork, IRoomAssignmentRepository RoomAssignmentRepository, IMapper mapper, IEmployeeRepository employeeRepository)
        {
            _unitOfWork = unitOfWork;
            _RoomAssignmentRepository = RoomAssignmentRepository;
            _mapper = mapper;
            _employeeRepository = employeeRepository;
        }

        public async Task<Unit> Handle(RemoveRoomAssignmentOwnershipRequest request, CancellationToken cancellationToken)
        {
            await _employeeRepository.EmployeeActiveCheck(request.EmployeeId);
            await _RoomAssignmentRepository.RemoveOwnershipRoom(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
