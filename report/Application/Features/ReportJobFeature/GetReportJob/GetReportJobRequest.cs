using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.GetReportJob
{
    public sealed record GetReportJobRequest(int Id) : IRequest<GetReportJobResponse>;
}
