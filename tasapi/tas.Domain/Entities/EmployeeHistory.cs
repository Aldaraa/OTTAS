using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class EmployeeHistory : BaseEntity
    {
        public int EmployeeId { get; set; }

        public string? Comment { get; set; }

        public DateTime EventDate { get; set; }

        public int? TerminationTypeId { get; set; }

        public string Action { get; set; }

    }
}
