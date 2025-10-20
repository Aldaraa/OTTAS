using AutoMapper;
using MediatR;
using tas.Application.Features.RoomFeature.FindAvailableRoom;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.FindAvailableByRoomId
{

    public sealed class FindAvailableByRoomIdHandler : IRequestHandler<FindAvailableByRoomIdRequest, FindAvailableByRoomIdResponse>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public FindAvailableByRoomIdHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<FindAvailableByRoomIdResponse> Handle(FindAvailableByRoomIdRequest request, CancellationToken cancellationToken)
        {
            return await _RoomRepository.FindAvailableByRoomId(request, cancellationToken);
        }
    }
}
