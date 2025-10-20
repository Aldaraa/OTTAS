using MediatR;
using tas.Application.Features.RoomFeature.FindAvailableByDates;
using tas.Domain.Common;

namespace tas.Application.Features.RoomFeature.FindAvailableByDates
{
    public sealed record FindAvailableByDatesRequest(DateTime startDate,
        DateTime endDate,
        int? CampId,
        int? RoomTypeId,
        int? Private,
        string? RoomNumber,
        int? bedCount,
        int employeeId) : IRequest<List<FindAvailableByDatesResponse>>;



}
