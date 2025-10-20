using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeStatusFeature.RoomBookingEmployee;

namespace tas.Application.Features.TransportScheduleFeature.GetMonthTransportSchedule
{
    public sealed record GetMonthTransportScheduleRequest(int? FromLocationId,
        int? ToLocationId,
        DateTime? ScheduleDate,
        string? TransportCode,
        int? TransportModeId) : IRequest<List<GetMonthTransportScheduleResponse>>;
}
