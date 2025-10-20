using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.StateFeature.CreateState
{
    public sealed record CreateStateRequest(string Code, string Description) : IRequest;
}
