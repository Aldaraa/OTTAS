using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.GetRoomAssignAvialable
{

    public sealed class GetRoomAssignAvialableHandler : IRequestHandler<GetRoomAssignAvialableRequest, List<GetRoomAssignAvialableResponse>>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public GetRoomAssignAvialableHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<List<GetRoomAssignAvialableResponse>> Handle(GetRoomAssignAvialableRequest request, CancellationToken cancellationToken)
        {
            return await _RoomRepository.GetRoomAssignAvialable(request, cancellationToken);
        }
    }
}
