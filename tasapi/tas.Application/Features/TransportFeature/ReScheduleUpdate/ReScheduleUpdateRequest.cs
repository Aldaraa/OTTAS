using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.ReScheduleUpdate
{
    public sealed record ReScheduleUpdateRequest(
        int ShiftId,
        int ScheduleId,
        int oldTransportId,
        int? DepartmentId,
        int? CostCodeId,
        int? ExistingScheduleIdNoShow,
        int? ReScheduleGoShow
        ) : IRequest<Unit>;


    

}
