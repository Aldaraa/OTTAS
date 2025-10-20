using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.GetDataRequest
{
    public sealed record GetDataRequestRequest(string datarequest, string key) : IRequest<GetDataRequestResponse>;
}
