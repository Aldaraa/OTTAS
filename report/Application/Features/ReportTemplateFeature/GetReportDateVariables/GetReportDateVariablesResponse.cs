using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportTemplateFeature.GetReportDateVariables
{
    public sealed record GetReportDateVariablesResponse
    {
        public List<string> DateVariables { get; set; }


    }

}
