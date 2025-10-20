using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ActiveTransportFeature.UpdateActiveTransport
{
    public sealed record UpdateActiveTransportRequest(
        int Id,
        string Code,
        string? ETD,
        string? ETA,
        int CarrierId,
        int Seats,
        DateTime StartDate,
        DateTime EndDate
        ) : IRequest;
}
