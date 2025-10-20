using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.CreateReportJobMonthly
{
    public sealed record CreateReportJobMonthlyRequest(string Name, string? Description, int reportTemplateId,
         List<int>? ColumnIds, List<CreateReportJobMonthlyParameter> Parameters, DateTime startDate, DateTime? endDate,
        List<string> subscriptionMails, List<string> months, List<int> days
        ) : IRequest;

    public sealed record CreateReportJobMonthlyParameter
    {
        public int ParameterId { get; set; }

        public string? ParameterValue { get; set; }

        public int? Days { get; set; }
    }
}
