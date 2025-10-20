using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeStatusFeature.RoomBookingEmployee;

namespace tas.Application.Features.TransportScheduleFeature.SearchTransportSchedule
{
    public sealed record SearchTransportScheduleRequest(string Code,  int Active) : IRequest<List<SearchTransportScheduleResponse>>;
}
