using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.CreateReportJobRuntime
{
    public sealed record CreateReportJobRuntimeRequest(string Name, string? Description, int reportTemplateId,
         List<int>? ColumnIds, List<CreateReportJobRuntimeParameter> Parameters, DateTime executeDate,
        List<string> subscriptionMails
        ) : IRequest;


    public sealed record CreateReportJobRuntimeParameter
    {
        public int ParameterId { get; set; }

        public string? ParameterValue { get; set; }

        public int? Days { get; set; }

    }
}
