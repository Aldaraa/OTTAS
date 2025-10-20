using MediatR;
using tas.Application.Features.RoomFeature.FindAvailableRoom;

namespace tas.Application.Features.RoomFeature.FindAvailableRoom
{
    public sealed record FindAvailableRoomRequest(
            DateTime startDate, 
            DateTime endDate, 
            int CampId,
            int? RoomTypeId,
            int? Private, 
            string? RoomNumber

        ) : IRequest<List<FindAvailableRoomResponse>>;
}
