using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.GetRoom
{

    public sealed class GetRoomHandler : IRequestHandler<GetRoomRequest, GetRoomResponse>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public GetRoomHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<GetRoomResponse> Handle(GetRoomRequest request, CancellationToken cancellationToken)
        {
            var room = await _RoomRepository.Get(request.Id, cancellationToken);
            return _mapper.Map<GetRoomResponse>(room);
        }
    }
}
