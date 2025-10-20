using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestLineManagerEmployee : BaseEntity
    {

        public int EmployeeId { get; set; }

        public int LineManagerId { get; set; }
    }
}
