using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.SetTransport
{
    public sealed record SetTransportRequest(List<int> EmpIds, int EventId, int InScheduleId, int OutScheduleId) : IRequest;
}
