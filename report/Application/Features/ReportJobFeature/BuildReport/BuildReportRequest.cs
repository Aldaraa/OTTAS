using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.BuildReport
{
    public sealed record BuildReportRequest(int reportTemplateId,
         List<int>? ColumnIds, List<BuildReportParameter> Parameters
        ) : IRequest<BuildReportResponse>;


    public sealed record BuildReportParameter
    {
        public int ParameterId { get; set; }

        public string? ParameterValue { get; set; }

        public int? Days { get; set; }
    }
}
