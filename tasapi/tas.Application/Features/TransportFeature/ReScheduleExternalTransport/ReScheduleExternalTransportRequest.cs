using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.ReScheduleExternalTransport
{
    public sealed record ReScheduleExternalTransportRequest(
        int ScheduleId,
        int oldTransportId,
        int? DepartmentId,
        int? CostCodeId
        ) : IRequest<Unit>;
}
