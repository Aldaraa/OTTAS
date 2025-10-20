using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.GetScheduleDetailTransport
{
    public sealed record GetScheduleDetailTransportRequest(int ScheduleId) : IRequest<List<GetScheduleDetailTransportResponse>>;
}
