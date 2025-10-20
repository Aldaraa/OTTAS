using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeStatusFeature.RoomBookingEmployee;

namespace tas.Application.Features.TransportScheduleFeature.GetScheduleBusstop
{
    public sealed record GetScheduleBusstopRequest(int ScheduleId) : IRequest<List<GetScheduleBusstopResponse>>;
}
