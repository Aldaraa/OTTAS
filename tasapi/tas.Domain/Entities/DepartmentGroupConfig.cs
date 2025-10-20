using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{

    public class DepartmentGroupConfig : BaseEntity
    {
        public int DepartmentId { get; set; }

        public int? EmployerId { get; set; }

        public int? GroupMasterId { get; set; }

        public int? GroupDetailId { get; set; }

    }
}
