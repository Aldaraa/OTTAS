using AutoMapper;
using MediatR;
using tas.Application.Features.RoomFeature.ActiveSearchRoom;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.ActiveSearchRoom
{

    public sealed class ActiveSearchRoomHandler : IRequestHandler<ActiveSearchRoomRequest, ActiveSearchRoomResponse>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public ActiveSearchRoomHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<ActiveSearchRoomResponse> Handle(ActiveSearchRoomRequest request, CancellationToken cancellationToken)
        {
            var Rooms = await _RoomRepository.StatusBetweenDates(request, cancellationToken);
            return Rooms;

        }
    }
}
