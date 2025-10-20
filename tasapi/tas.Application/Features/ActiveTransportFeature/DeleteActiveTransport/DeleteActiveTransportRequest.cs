using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ActiveTransportFeature.DeleteActiveTransport
{
    public sealed record DeleteActiveTransportRequest(int Id) : IRequest;
}
