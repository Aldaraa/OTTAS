using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.GetAllActiveRoomAssignment
{

    public sealed class GetAllActiveRoomAssignmentHandler : IRequestHandler<GetAllActiveRoomAssignmentRequest, List<GetAllActiveRoomAssignmentResponse>>
    {
        private readonly IRoomAssignmentRepository _RoomAssignmentRepository;
        private readonly IMapper _mapper;

        public GetAllActiveRoomAssignmentHandler(IRoomAssignmentRepository RoomAssignmentRepository, IMapper mapper)
        {
            _RoomAssignmentRepository = RoomAssignmentRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllActiveRoomAssignmentResponse>> Handle(GetAllActiveRoomAssignmentRequest request, CancellationToken cancellationToken)
        {
                return await _RoomAssignmentRepository.GetAllActiveRooms(request, cancellationToken);
             
           

        }
    }
}
