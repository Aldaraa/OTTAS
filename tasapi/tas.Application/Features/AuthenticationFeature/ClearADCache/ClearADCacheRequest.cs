using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.AuthenticationFeature.ClearADCache
{
    public sealed record ClearADCacheRequest(string AdAccount) : IRequest;
}
