using MediatR;
using tas.Application.Features.PositionFeature.GetAllPosition;

namespace tas.Application.Features.EmployeeStatusFeature.GetDateRangeStatus
{
    public sealed record GetDateRangeStatusRequest(int EmployeeId, DateTime StartDate, int DurationMonth) : IRequest<List<GetDateRangeStatusResponse>>;
}
