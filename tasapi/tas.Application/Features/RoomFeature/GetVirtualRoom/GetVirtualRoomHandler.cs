using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.GetVirtualRoom
{

    public sealed class GetVirtualRoomHandler : IRequestHandler<GetVirtualRoomRequest, GetVirtualRoomResponse>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public GetVirtualRoomHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<GetVirtualRoomResponse> Handle(GetVirtualRoomRequest request, CancellationToken cancellationToken)
        {
            return await _RoomRepository.GetVirtualRoomId(cancellationToken);
        }
    }
}
