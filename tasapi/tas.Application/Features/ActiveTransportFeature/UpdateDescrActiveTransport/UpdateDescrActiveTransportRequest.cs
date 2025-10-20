using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ActiveTransportFeature.UpdateDescrActiveTransport
{
    public sealed record UpdateDescrActiveTransportRequest(
        int Id,
       string description
            
        ) : IRequest;
}
