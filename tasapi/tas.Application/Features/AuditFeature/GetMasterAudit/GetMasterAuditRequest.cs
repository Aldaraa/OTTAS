using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.AuditFeature.GetMasterAudit
{
    public sealed record GetMasterAuditRequest(string TableName, int? recordId, DateTime? startDate, DateTime? endDate) : IRequest<List<GetMasterAuditResponse>>;
}
