using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.AuditFeature.GetRoomAudit
{
    public sealed record GetRoomAuditRequest(List<int> employeeIds, DateTime startDate, DateTime endDate) : IRequest<GetRoomAuditResponse>;
}
