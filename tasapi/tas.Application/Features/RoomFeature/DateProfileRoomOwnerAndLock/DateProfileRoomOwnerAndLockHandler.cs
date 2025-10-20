using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomOwnerAndLockFeature.DateProfileRoomOwnerAndLock
{

    public sealed class DateProfileRoomOwnerAndLockHandler : IRequestHandler<DateProfileRoomOwnerAndLockRequest, DateProfileRoomOwnerAndLockResponse>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public DateProfileRoomOwnerAndLockHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<DateProfileRoomOwnerAndLockResponse> Handle(DateProfileRoomOwnerAndLockRequest request, CancellationToken cancellationToken)
        {
            return await _RoomRepository.GetRoomOwnerAndLockDateProfile(request, cancellationToken);

        }
    }
}
