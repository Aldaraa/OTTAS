using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportTemplateFeature.GetAllReportTemplate
{
    public sealed record GetAllReportTemplateRequest(int? status) : IRequest<List<GetAllReportTemplateResponse>>;
}
