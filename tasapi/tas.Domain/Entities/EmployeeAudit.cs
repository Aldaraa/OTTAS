using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class EmployeeAudit : BaseEntity
    {
        public int? UserId { get; set; }
        public int? EmployeeId { get; set; }
        public string? Type { get; set; }
        public DateTime? DateTime { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? AffectedColumns { get; set; }
        public string? PrimaryKey { get; set; }

    }
}
