using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.TransportScheduleInfo
{
    public sealed record TransportScheduleInfoRequest(int Id) : IRequest<TransportScheduleInfoResponse>;
}
