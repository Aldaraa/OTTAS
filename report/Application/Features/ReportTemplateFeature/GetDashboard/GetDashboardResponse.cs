using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportTemplateFeature.GetDashboard
{
    public sealed record GetDashboardResponse
    {
        public int ALLReportTemplateCount { get; set; }
        public int ALLReportScheduleCount { get; set; }

        public int ActiveReportTemplateCount { get; set; }
        public int ActiveReportScheduleCount { get; set; }

        public int TodayReportScheduleCount { get; set; }

        public List<GetDashboardScheduleTypes> ScheduleTypesData { get; set; }

        public List<GetDashboardTemplateTypes> TemplateTypeData { get; set; }

        public List<UpComningSchedule> UpComningScheduleData { get; set; }


    }

    public sealed record GetDashboardScheduleTypes
    { 
        public string ScheduleType { get; set; }

        public int? AllCount { get; set; }

        public int? ActiveCount { get; set; }


    }


    public sealed record GetDashboardTemplateTypes
    {
        public string TemplateName { get; set; }
        public int AllCount { get; set; }
    }


    public sealed record UpComningSchedule
    {
        public string UpcomingTypes { get; set; }
        public int ScheduleCount { get; set; }
    }
}
