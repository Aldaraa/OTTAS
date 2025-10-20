using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.DateProfileRoomDetail
{

    public sealed class DateProfileRoomDetailHandler : IRequestHandler<DateProfileRoomDetailRequest, List<DateProfileRoomDetailResponse>>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public DateProfileRoomDetailHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<List<DateProfileRoomDetailResponse>> Handle(DateProfileRoomDetailRequest request, CancellationToken cancellationToken)
        {
            return await _RoomRepository.GetRoomDetailDateProfile(request, cancellationToken);

        }
    }
}
