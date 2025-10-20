using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SafeModeTransportFeature.DeleteTransport
{
    public sealed record DeleteTransportRequest(
        int Id
        ) : IRequest;
}
