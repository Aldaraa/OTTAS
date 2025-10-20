using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.GetReportJobLog
{
    public sealed record GetReportJobLogRequest(int Id) : IRequest<List<GetReportJobLogResponse>>;
}
