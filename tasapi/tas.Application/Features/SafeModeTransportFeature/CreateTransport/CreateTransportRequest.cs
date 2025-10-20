using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SafeModeTransportFeature.CreateTransport
{
    public sealed record CreateTransportRequest(
        int EmployeeId,
        int ScheduleId,
        int DepartmentId,
        int PositionId,
        int EmployerId,
        int CostCodeId
        ) : IRequest;
}
