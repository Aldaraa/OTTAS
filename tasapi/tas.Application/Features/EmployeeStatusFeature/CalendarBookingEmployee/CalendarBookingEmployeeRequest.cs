using MediatR;
using tas.Application.Features.PositionFeature.GetAllPosition;

namespace tas.Application.Features.EmployeeStatusFeature.CalendarBookingEmployee
{
    public sealed record CalendarBookingEmployeeRequest(int EmployeeId, DateTime CurrentDate) : IRequest<List<CalendarBookingEmployeeResponse>>;
}
