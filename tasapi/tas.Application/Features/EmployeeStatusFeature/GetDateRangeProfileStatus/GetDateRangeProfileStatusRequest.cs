using MediatR;
using tas.Application.Features.PositionFeature.GetAllPosition;

namespace tas.Application.Features.EmployeeProfileStatusFeature.GetDateRangeProfileStatus
{
    public sealed record GetDateRangeProfileStatusRequest(int EmployeeId, DateTime StartDate, DateTime EndDate) : IRequest<List<GetDateRangeProfileStatusResponse>>;
}
