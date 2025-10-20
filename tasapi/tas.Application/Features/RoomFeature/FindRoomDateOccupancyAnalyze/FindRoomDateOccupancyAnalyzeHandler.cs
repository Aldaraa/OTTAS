using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.FindRoomDateOccupancyAnalyze
{

    public sealed class FindRoomDateOccupancyAnalyzeHandler : IRequestHandler<FindRoomDateOccupancyAnalyzeRequest, FindRoomDateOccupancyAnalyzeResponse>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public FindRoomDateOccupancyAnalyzeHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<FindRoomDateOccupancyAnalyzeResponse> Handle(FindRoomDateOccupancyAnalyzeRequest request, CancellationToken cancellationToken)
        {
            return await _RoomRepository.FindRoomDateOccupancyAnalyze(request, cancellationToken);
        }
    }
}
