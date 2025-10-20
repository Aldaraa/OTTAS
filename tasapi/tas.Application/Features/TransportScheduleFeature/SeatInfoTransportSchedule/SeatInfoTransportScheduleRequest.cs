using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeStatusFeature.RoomBookingEmployee;
using tas.Domain.Common;

namespace tas.Application.Features.TransportScheduleFeature.SeatInfoTransportSchedule
{
    public sealed record SeatInfoTransportScheduleRequest(int ScheduleId) :  IRequest<SeatInfoTransportScheduleResponse>;

}



