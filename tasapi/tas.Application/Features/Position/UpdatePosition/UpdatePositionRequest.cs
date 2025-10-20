using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.PositionFeature.UpdatePosition
{
    public sealed record UpdatePositionRequest(int Id, string Code, string Description) : IRequest;
}
