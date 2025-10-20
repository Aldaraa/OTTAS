using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.MonthStatusRoomOwner
{

    public sealed class MonthStatusRoomOwnerHandler : IRequestHandler<MonthStatusRoomOwnerRequest, List<MonthStatusRoomOwnerResponse>>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public MonthStatusRoomOwnerHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<List<MonthStatusRoomOwnerResponse>> Handle(MonthStatusRoomOwnerRequest request, CancellationToken cancellationToken)
        {
            return await _RoomRepository.GetRoomMonthStatusOwner(request, cancellationToken);
        }
    }
}
