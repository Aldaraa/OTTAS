using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExectute;

namespace tas.Application.Features.RosterExecuteFeature.BulkRosterExecute
{
    public sealed record BulkRosterExecuteRequest(DateTime StartDate, int DurationMonth, List<BulkRosterExecuteEmployee> Employees, int FlightGroupMasterId ) : IRequest<BulkRosterExecuteResponse>;


    public sealed record BulkRosterExecuteEmployee(int EmployeeId, int RosterId, int CostCodeId, int PositionId, int DepartmentId, int? EmployerId ) : IRequest;
}
