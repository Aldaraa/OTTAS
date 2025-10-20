using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.GetReportJob
{
    public sealed record GetReportJobResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Description { get; set; }
        public int? Active { get; set; }

        public string ScheduleType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }


        public string? reportTemplateCode { get; set; }
        public int? reportTemplateId { get; set; }

        public string? Command { get; set; }
        public List<string?> subscriptionMails { get; set; }

        public List<JobParameter> Parameters { get; set; }

        public List<JobJobColumns> Columns { get; set; }
    }


    public sealed record JobParameter
    {
        public int Id { get; set; }
        public int ParameterId { get; set; }

        public string? Caption { get; set; }

        public string? Descr { get; set; }



        public string? Component { get; set; }

        public string? FieldName { get; set; }


        public string? ParameterValue { get; set; }

        public int? Days { get; set; }
    }
    
    public sealed record JobJobColumns
    {
        public int ColumnId { get; set; }

        public string? FieldName { get; set; }

        public string? Caption { get; set; }

    }
}
