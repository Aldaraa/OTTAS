using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.RemoveExternalTransport
{
    public sealed record RemoveExternalTransportRequest(
        int TransportId
        ) : IRequest<Unit>;


    

}
