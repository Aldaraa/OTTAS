using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.UpdateReportJobWeekly
{
    public sealed record UpdateReportJobWeeklyRequest(int Id, string Name, string? Description, int reportTemplateId,
         List<int>? ColumnIds, List<UpdateReportJobWeeklyParameter> Parameters, DateTime startDate, DateTime? endDate,
        List<string> subscriptionMails, int recureEvery, List<string> days
        ) : IRequest;

    public sealed record UpdateReportJobWeeklyParameter
    {
        public int ParameterId { get; set; }

        public string? ParameterValue { get; set; }

        public int? Days { get; set; }
    }
}
