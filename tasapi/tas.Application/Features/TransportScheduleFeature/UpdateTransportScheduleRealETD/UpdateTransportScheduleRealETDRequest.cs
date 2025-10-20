using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.UpdateTransportScheduleRealETD
{
    public sealed record UpdateTransportScheduleRealETDRequest(
        int Id,
        string RealETD,
        string? Remark

        ) : IRequest;
}
