using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SysRoleEmployeeReportTemplate : BaseEntity
    {
        public int EmployeeId { get; set; }

        public int ReportTemplateId { get; set; }
    }
}
