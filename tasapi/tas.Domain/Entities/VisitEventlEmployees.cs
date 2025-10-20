using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class VisitEventlEmployees : BaseEntity
    {
        public int EventId { get; set; }
        public int EmployeeId { get; set; }


    }
}
