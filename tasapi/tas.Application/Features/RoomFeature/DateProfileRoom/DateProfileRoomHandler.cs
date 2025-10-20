using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.DateProfileRoom
{

    public sealed class DateProfileRoomHandler : IRequestHandler<DateProfileRoomRequest, DateProfileRoomResponse>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public DateProfileRoomHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<DateProfileRoomResponse> Handle(DateProfileRoomRequest request, CancellationToken cancellationToken)
        {
            return await _RoomRepository.GetRoomDateProfile(request, cancellationToken);

        }
    }
}
