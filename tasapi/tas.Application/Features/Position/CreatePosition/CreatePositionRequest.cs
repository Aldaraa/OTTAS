using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.PositionFeature.CreatePosition
{
    public sealed record CreatePositionRequest(string Code, string Description) : IRequest;
}
