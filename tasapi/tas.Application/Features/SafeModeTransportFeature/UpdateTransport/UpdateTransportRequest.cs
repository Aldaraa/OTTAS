using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SafeModeTransportFeature.UpdateTransport
{
    public sealed record UpdateTransportRequest(
        int TransportId,
        int ScheduleId
        ) : IRequest;
}
