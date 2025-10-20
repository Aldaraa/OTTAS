using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.RemoveTransport
{
    public sealed record RemoveTransportRequest(
        int startScheduleId,
        int endScheduleId,
        int shiftId,
        int? FirstScheduleNoShow,
        int? LastScheduleNoShow
        ) : IRequest<Unit>;


    

}
