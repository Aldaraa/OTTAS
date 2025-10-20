using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.MonthStatusRoom
{

    public sealed class MonthStatusRoomHandler : IRequestHandler<MonthStatusRoomRequest, MonthStatusRoomResponse>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public MonthStatusRoomHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<MonthStatusRoomResponse> Handle(MonthStatusRoomRequest request, CancellationToken cancellationToken)
        {
            return await _RoomRepository.GetRoomMonthStatus(request, cancellationToken);
        }
    }
}
