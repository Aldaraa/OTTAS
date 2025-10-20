using AutoMapper;
using MediatR;
using tas.Application.Features.RoomFeature.FindAvailableRoom;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.FindAvailableRoom
{

    public sealed class FindAvailableRoomHandler : IRequestHandler<FindAvailableRoomRequest, List<FindAvailableRoomResponse>>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public FindAvailableRoomHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<List<FindAvailableRoomResponse>> Handle(FindAvailableRoomRequest request, CancellationToken cancellationToken)
        {
            var Rooms = await _RoomRepository.FindAvailableRooms(request, cancellationToken);
            return Rooms;

        }
    }
}
