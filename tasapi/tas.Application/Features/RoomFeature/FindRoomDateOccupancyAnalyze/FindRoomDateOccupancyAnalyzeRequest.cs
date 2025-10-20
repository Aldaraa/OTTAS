using MediatR;
using tas.Application.Features.RoomFeature.FindRoomDateOccupancyAnalyze;

namespace tas.Application.Features.RoomFeature.FindRoomDateOccupancyAnalyze
{
    public sealed record FindRoomDateOccupancyAnalyzeRequest(
            DateTime StartDate,
            DateTime EndDate,
            int RoomId 

        ) : IRequest<FindRoomDateOccupancyAnalyzeResponse>;
}
