using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.TestReportJob
{
    public sealed record TestReportJobRequest(int Id
        ) : IRequest<TestReportJobResponse>;
}
