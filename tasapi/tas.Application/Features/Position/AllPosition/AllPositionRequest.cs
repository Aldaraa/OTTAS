using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.PositionFeature.AllPosition
{
    public sealed record AllPositionRequest(int? status) :  IRequest<List<AllPositionResponse>>;

}
