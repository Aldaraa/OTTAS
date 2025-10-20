using Application.Common.Utils;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportTemplateFeature.DateSimulation
{
    public sealed record DateSimulationRequest(string reportDateType, int days) : IRequest<DateTime>;
}
