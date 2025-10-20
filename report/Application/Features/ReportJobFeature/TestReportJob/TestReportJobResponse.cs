using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.TestReportJob
{

    public sealed record TestReportJobResponse
    {
        public List<TestReportJobResponseData> ExcelFiles { get; set; }

        public string ReportName  { get; set; }

    }

    public sealed record TestReportJobResponseData
    {
        public byte[] ExcelData { get; set; }

        public string? Filename { get; set; }
    }
}
