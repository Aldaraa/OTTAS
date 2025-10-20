using MediatR;
using tas.Application.Features.RoomFeature.GetRoomAssignAvialable;
using tas.Domain.Common;

namespace tas.Application.Features.RoomFeature.GetRoomAssignAvialable
{
    public sealed record GetRoomAssignAvialableRequest(
        int? CampId,
        int? RoomTypeId,
        int? Private,
        string? RoomNumber,
        int? BedCount,
        DateTime startDate,
        int employeeId) : IRequest<List<GetRoomAssignAvialableResponse>>;

}
