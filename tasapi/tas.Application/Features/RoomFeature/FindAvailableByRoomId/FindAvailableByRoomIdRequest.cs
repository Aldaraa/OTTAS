using MediatR;
using tas.Application.Features.RoomFeature.FindAvailableRoom;

namespace tas.Application.Features.RoomFeature.FindAvailableByRoomId
{
    public sealed record FindAvailableByRoomIdRequest(
            DateTime startDate, 
            DateTime endDate, 
            int RoomId,
            int EmployeeId

        ) : IRequest<FindAvailableByRoomIdResponse>;
}
