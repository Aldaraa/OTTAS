using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class StatusChangesEmployeeRequest : BaseEntity
    {
        public int EmployeeId { get; set; }
        public DateTime? EventDate { get; set; }

        public string? Comment { get; set; }

        public int? TerminationTypeId { get; set; }

        public string? StatusType { get; set; }

    }
}
