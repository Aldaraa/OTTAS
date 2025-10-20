using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.KillSession
{
    public sealed record KillSessionRequest(int killId) : IRequest<Unit>;
}
