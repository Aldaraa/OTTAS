using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExectutePreview;

namespace tas.Application.Features.RosterExecuteFeature.BulkRosterExecutePreview
{
    public sealed record BulkRosterExecutePreviewRequest(DateTime StartDate, int DurationMonth, List<BulkRosterExecutePreviewEmployee> Employees ) : IRequest<List<BulkRosterExecutePreviewResponse>>;


    public sealed record BulkRosterExecutePreviewEmployee(int EmployeeId, int RosterId,  int CostCodeId, int PositionId, int DepartmentId ) : IRequest;
}
