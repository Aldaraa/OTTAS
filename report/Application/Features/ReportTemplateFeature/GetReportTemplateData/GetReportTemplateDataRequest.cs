using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportTemplateFeature.GetReportTemplateData
{
    public sealed record GetReportTemplateDataRequest(int templateId) : IRequest<GetReportTemplateDataResponse>;
}
