using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.UpdateReportJobDaily
{
    public sealed record UpdateReportJobDailyRequest(int Id, string Name, string? Description, int reportTemplateId,
         List<int>? ColumnIds, List<UpdateReportJobDailyParameter> Parameters, DateTime startDate, DateTime? endDate,
        List<string> subscriptionMails, int recureEvery
        ) : IRequest;


    public sealed record UpdateReportJobDailyParameter
    {
        public int ParameterId { get; set; }

        public string? ParameterValue { get; set; }

        public int? Days { get; set; }
    }
}
