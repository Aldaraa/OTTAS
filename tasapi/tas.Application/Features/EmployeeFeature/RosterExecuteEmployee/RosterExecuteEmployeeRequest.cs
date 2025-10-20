using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.RosterExecuteEmployee
{
    public sealed record RosterExecuteEmployeeRequest(
        int EmployeeId,
        DateTime StartDate,
        int MonthDuration,
        int RosterId,
        int? PositionId,
        int DepartmentId,
        int CostCodeId,
        int EmployerId,
        int FlightGroupMasterId,
       // int LocationId,
        bool Confirmed = false
        ) : IRequest<List<RosterExecuteEmployeeResponse>>;
}
