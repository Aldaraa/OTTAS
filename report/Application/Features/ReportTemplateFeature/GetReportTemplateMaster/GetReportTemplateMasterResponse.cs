using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportTemplateFeature.GetReportTemplateMaster
{
    public sealed record GetReportTemplateMasterResponse
    {
        public List<string> Weekdays { get; set; }

        public List<string> Months { get; set; }

        public List<string> JobType { get; set; }


    }

}
