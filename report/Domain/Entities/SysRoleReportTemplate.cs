using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SysRoleReportTemplate : BaseEntity
    {
        public int RoleId { get; set; }

        public int ReportTemplateId { get; set; }
    }
}
