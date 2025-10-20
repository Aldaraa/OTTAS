using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportTemplateFeature.GetAllReportTemplate
{
    public sealed record GetAllReportTemplateResponse 
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        public string? Description { get; set; }
        public int? Active { get; set; }
        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public int ScheduleCount { get; set; }
    }
}
