using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.StateFeature.UpdateState
{
    public sealed record UpdateStateRequest(int Id, string Code, string Description) : IRequest;
}
