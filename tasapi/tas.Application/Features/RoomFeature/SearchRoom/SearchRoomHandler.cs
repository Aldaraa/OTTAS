using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.SearchRoom
{

    public sealed class SearchRoomHandler : IRequestHandler<SearchRoomRequest, SearchRoomResponse>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public SearchRoomHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<SearchRoomResponse> Handle(SearchRoomRequest request, CancellationToken cancellationToken)
        {
            var room = await _RoomRepository.SearchRoom(request, cancellationToken);
            return room;
        }
    }
}
