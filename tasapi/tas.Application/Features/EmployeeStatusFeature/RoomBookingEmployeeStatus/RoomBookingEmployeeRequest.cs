using MediatR;
using tas.Application.Features.PositionFeature.GetAllPosition;

namespace tas.Application.Features.EmployeeStatusFeature.RoomBookingEmployee
{
    public sealed record RoomBookingEmployeeRequest(int EmployeeId, DateTime StartDate, DateTime EndDate) : IRequest<List<RoomBookingEmployeeResponse>>;
}
