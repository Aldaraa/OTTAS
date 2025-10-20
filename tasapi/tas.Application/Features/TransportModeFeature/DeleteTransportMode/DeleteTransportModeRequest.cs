using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportModeFeature.DeleteTransportMode
{
    public sealed record DeleteTransportModeRequest(int Id) : IRequest;
}
