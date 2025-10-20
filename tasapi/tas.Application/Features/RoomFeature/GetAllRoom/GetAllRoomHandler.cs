using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.GetAllRoom
{

    public sealed class GetAllRoomHandler : IRequestHandler<GetAllRoomRequest, List<GetAllRoomResponse>>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public GetAllRoomHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllRoomResponse>> Handle(GetAllRoomRequest request, CancellationToken cancellationToken)
        {
                var users = await _RoomRepository.GetAll(request, cancellationToken);
                return _mapper.Map<List<GetAllRoomResponse>>(users);
           

        }
    }
}
