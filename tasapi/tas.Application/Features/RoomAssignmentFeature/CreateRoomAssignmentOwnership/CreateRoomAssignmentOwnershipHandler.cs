using AutoMapper;
using MediatR;
using tas.Application.Features.RoomAssignmentFeature.CreateRoomAssignmentOwnership;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.CreateRoomAssignmentOwnership
{
    public sealed class CreateRoomAssignmentOwnershipHandler : IRequestHandler<CreateRoomAssignmentOwnershipRequest, List<CreateRoomAssignmentOwnershipResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomAssignmentRepository _RoomAssignmentRepository;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;

        public CreateRoomAssignmentOwnershipHandler(IUnitOfWork unitOfWork, IRoomAssignmentRepository RoomAssignmentRepository, IMapper mapper, IEmployeeRepository employeeRepository)
        {
            _unitOfWork = unitOfWork;
            _RoomAssignmentRepository = RoomAssignmentRepository;
            _mapper = mapper;
            _employeeRepository = employeeRepository;
        }

        public async Task<List<CreateRoomAssignmentOwnershipResponse>> Handle(CreateRoomAssignmentOwnershipRequest request, CancellationToken cancellationToken)
        {
            await _employeeRepository.EmployeeActiveCheck(request.EmployeeId);
          //  await _RoomAssignmentRepository.RoomOwnerCheck(request.RoomId, cancellationToken);
            var data = await _RoomAssignmentRepository.SaveOwnershipRoom(request, cancellationToken);
            if (data.Count == 0)
            {
                await _unitOfWork.Save(cancellationToken);
            }
            return data;
        }
    }
}
