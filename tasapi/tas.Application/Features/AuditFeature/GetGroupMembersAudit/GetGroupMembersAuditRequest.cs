using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.AuditFeature.GetGroupMembersAudit
{
    public sealed record GetGroupMembersAuditRequest(int EmployeeId, int GroupMasterId) : IRequest<List<GetGroupMembersAuditResponse>>;
}
