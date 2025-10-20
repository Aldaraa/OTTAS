using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.UpdateReportJobRuntime
{
    public sealed record UpdateReportJobRuntimeRequest(int Id, string Name, string? Description, int reportTemplateId,
         List<int>? ColumnIds, List<UpdateReportJobRuntimeParameter> Parameters, DateTime executeDate,
        List<string> subscriptionMails
        ) : IRequest;


    public sealed record UpdateReportJobRuntimeParameter
    {
        public int ParameterId { get; set; }

        public string? ParameterValue { get; set; }

        public int? Days { get; set; }
    }
}
