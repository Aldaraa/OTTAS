using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.BuildReport
{
    public sealed record BuildReportResponse
    {
        public List<BuildReportResponseExcelData> ExcelFiles { get; set; }

        public string ReportName { get; set; }


        

    }

    public sealed record BuildReportResponseExcelData
    { 
        public byte[] ExcelData { get; set; }

        public string? Filename { get; set; }
    }


    public sealed record ExecuteDataResponse
    {
        public List<string> Files { get; set; }

        public string ReportName { get; set; }
    }


}
