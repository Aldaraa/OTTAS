using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.RosterExecutePreviewEmployee
{
    public sealed record RosterExecutePreviewEmployeeRequest(
        int EmployeeId,
        DateTime StartDate,
        int MonthDuration,
        int RosterId,
       // int? RoomId,
        int? DepartmentId,
        int? CostCodeId,
        int? EmployerId,
        int FlightGroupMasterId
      //  int LocationId
        ) : IRequest<RosterExecutePreviewEmployeeResponse>;
}
