using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.AssignRoomDateOccupancyAnalyze
{

    public sealed class AssignRoomDateOccupancyAnalyzeHandler : IRequestHandler<AssignRoomDateOccupancyAnalyzeRequest, AssignRoomDateOccupancyAnalyzeResponse>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public AssignRoomDateOccupancyAnalyzeHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<AssignRoomDateOccupancyAnalyzeResponse> Handle(AssignRoomDateOccupancyAnalyzeRequest request, CancellationToken cancellationToken)
        {
            return await _RoomRepository.AssignRoomDateOccupancyAnalyze(request, cancellationToken);
        }
    }
}
