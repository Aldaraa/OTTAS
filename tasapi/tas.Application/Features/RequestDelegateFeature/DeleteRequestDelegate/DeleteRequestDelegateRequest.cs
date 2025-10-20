using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDelegateFeature.DeleteRequestDelegate
{
    public sealed record DeleteRequestDelegateRequest(int Id) : IRequest;
}
