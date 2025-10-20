using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.UpdateReportJobMonthly
{
    public sealed record UpdateReportJobMonthlyRequest(int Id, string Name, string? Description, int reportTemplateId,
         List<int>? ColumnIds, List<UpdateReportJobMonthlyParameter> Parameters, DateTime startDate, DateTime? endDate,
        List<string> subscriptionMails, List<string> months, List<int> days
        ) : IRequest;

    public sealed record UpdateReportJobMonthlyParameter
    {
        public int ParameterId { get; set; }

        public string? ParameterValue { get; set; }

        public int? Days { get; set; }
    }
}
