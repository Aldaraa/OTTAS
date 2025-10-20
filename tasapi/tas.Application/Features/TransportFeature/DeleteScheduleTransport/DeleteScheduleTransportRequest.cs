using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.DeleteScheduleTransport
{
    public sealed record DeleteScheduleTransportRequest(List<int> ScheduleIds) : IRequest;
}
