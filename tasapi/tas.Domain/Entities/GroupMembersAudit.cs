using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public class GroupMembersAudit : BaseEntity
    {
        public int? GroupDetailId { get; set; }

        public int? GroupMasterId { get; set; }

        public int? EmployeeId { get; set; }

        public string? Action { get; set; }
    }
}
