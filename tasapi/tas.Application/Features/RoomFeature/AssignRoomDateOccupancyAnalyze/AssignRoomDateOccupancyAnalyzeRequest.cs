using MediatR;
using tas.Application.Features.RoomFeature.AssignRoomDateOccupancyAnalyze;

namespace tas.Application.Features.RoomFeature.AssignRoomDateOccupancyAnalyze
{
    public sealed record AssignRoomDateOccupancyAnalyzeRequest(
            DateTime StartDate,

            DateTime EndDate,
            int RoomId 

        ) : IRequest<AssignRoomDateOccupancyAnalyzeResponse>;
}
