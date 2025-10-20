using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SafeModeEmployeeStatusFeature.CreateEmployeeStatus
{
    public sealed record CreateEmployeeStatusRequest(
        int EmployeeId,
        int ShiftId,
        DateTime EventDate,
        int DepartmentId,
        int PositionId,
        int EmployerId,
        int CostCodeId,
        int? RoomId

        ) : IRequest;
}
