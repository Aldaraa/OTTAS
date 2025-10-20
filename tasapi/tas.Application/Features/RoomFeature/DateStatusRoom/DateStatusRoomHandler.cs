using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.DateStatusRoom
{

    public sealed class DateStatusRoomHandler : IRequestHandler<DateStatusRoomRequest, DateStatusRoomResponse>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public DateStatusRoomHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<DateStatusRoomResponse> Handle(DateStatusRoomRequest request, CancellationToken cancellationToken)
        {
            return await _RoomRepository.GetRoomDateStatus(request, cancellationToken);

        }
    }
}
