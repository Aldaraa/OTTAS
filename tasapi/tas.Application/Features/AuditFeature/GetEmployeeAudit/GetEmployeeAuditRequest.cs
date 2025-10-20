using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.AuditFeature.GetEmployeeAudit
{
    public sealed record GetEmployeeAuditRequest(int EmployeeId, DateTime? startDate, DateTime? endDate) : IRequest<GetEmployeeAuditResponse>;
}
                
