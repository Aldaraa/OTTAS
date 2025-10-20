using AutoMapper;
using MediatR;
using tas.Application.Features.RoomAssignmentFeature.CreateRoomAssignment;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.CreateRoomAssignment
{
    public sealed class CreateRoomAssignmentHandler : IRequestHandler<CreateRoomAssignmentRequest, CreateRoomAssignmentResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomAssignmentRepository _RoomAssignmentRepository;
        private readonly IMapper _mapper;

        public CreateRoomAssignmentHandler(IUnitOfWork unitOfWork, IRoomAssignmentRepository RoomAssignmentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RoomAssignmentRepository = RoomAssignmentRepository;
            _mapper = mapper;
        }

        public async Task<CreateRoomAssignmentResponse> Handle(CreateRoomAssignmentRequest request, CancellationToken cancellationToken)
        {   

            var data =  await _RoomAssignmentRepository.SaveTemporaryRoom(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return data;
        }
    }
}
