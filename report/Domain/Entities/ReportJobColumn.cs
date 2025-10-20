using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public sealed class ReportJobColumn : BaseEntity
    {
        public int ColumnId { get; set; }
        public int ReportJobId { get; set; }
    }
}
