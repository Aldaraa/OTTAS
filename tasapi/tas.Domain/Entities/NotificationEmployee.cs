using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class NotificationEmployee : BaseEntity
    {
        public string NotifIndex { get; set; }

        public int EmployeeId { get; set; }

        public int ViewStatus { get; set; } = 0;
     }
}
