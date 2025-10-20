using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.AddTravelTransport
{
    public sealed record AddTravelTransportRequest(
        int EmployeeId,
        int inScheduleId, 
        int outScheduleId,
        int CampId,
        int RoomId, 
        int ShiftId, 
        int DepartmentId,
        int PositionId, 
        int EmployerId, 
        int CostCodeId,
        int? inScheduleGoShow,
        int? outScheduleGoShow



        ) : IRequest;
}
