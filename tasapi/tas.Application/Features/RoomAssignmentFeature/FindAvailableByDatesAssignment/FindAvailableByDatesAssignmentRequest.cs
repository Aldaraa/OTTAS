using MediatR;
using tas.Application.Features.RoomAssignmentFeature.FindAvailableByDatesAssignment;
using tas.Domain.Common;

namespace tas.Application.Features.RoomAssignmentFeature.FindAvailableByDatesAssignment
{
    public sealed record FindAvailableByDatesAssignmentRequest(DateTime startDate,
        DateTime endDate,
        int? CampId,
        int? RoomTypeId,
        int? Private,
        int? BedCount,
        string? RoomNumber,
        int employeeId) : IRequest<List<FindAvailableByDatesAssignmentResponse>>;



}
