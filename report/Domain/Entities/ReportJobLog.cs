using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public sealed class ReportJobLog : BaseEntity
    {
        public DateTime? ExecuteDate { get; set; }
        public string? ExecuteStatus { get; set; }
        public string? Descr { get; set; }
        public int? ReportJobId { get; set; }
    }
}
