using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.GetAllReportJob
{
    public sealed record GetAllReportJobResponse
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        public string? Description { get; set; }
        public int? Active { get; set; }

        public string? ScheduleType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? ScheduleCommandParameter {get; set;}

        public string? ReportTemplateName { get; set; }

        public DateTime? nextExecuteDate { get; set; }


        public DateTime? lastExecuteDate { get; set; }

        public string? lastExecuteStatus { get; set; }

        public string? CreatedUser { get; set; }

        public DateTime? CreatedDate { get; set; }






    }

}
