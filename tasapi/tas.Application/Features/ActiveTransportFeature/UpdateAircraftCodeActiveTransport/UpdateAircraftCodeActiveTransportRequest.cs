using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ActiveTransportFeature.UpdateAircraftCodeActiveTransport
{
    public sealed record UpdateAircraftCodeActiveTransportRequest(
        int Id,
       string AircraftCode

        ) : IRequest;
}
