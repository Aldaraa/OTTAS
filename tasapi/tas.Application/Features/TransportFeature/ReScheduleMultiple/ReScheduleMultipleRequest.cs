using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.ReScheduleMultiple
{
    public sealed record ReScheduleMultipleRequest(
        int ShiftId ,
        int ScheduleId,
        List<int> TransportIds
        ) : IRequest<List<ReScheduleMultipleResponse>>;


    

}
