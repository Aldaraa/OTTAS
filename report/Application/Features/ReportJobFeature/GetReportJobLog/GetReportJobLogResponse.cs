using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.GetReportJobLog
{
    public sealed record GetReportJobLogResponse
    {
        public int Id { get; set; }
        public DateTime? ExecuteDate { get; set; }
        public string? ExecuteStatus { get; set; }
        public string? Descr { get; set; }
        public int? ReportJobId { get; set; }

    }


    
}
