using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.PositionFeature.DeletePosition
{
    public sealed record DeletePositionRequest(int Id) : IRequest;
}
