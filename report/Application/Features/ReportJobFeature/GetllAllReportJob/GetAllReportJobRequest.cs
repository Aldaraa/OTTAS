using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.GetAllReportJob
{
    public sealed record GetAllReportJobRequest(int? templateId, string? keyword) : IRequest<List<GetAllReportJobResponse>>;
}
