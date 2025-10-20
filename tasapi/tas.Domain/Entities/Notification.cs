using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class Notification : BaseEntity
    {
        public string? Description { get; set; }

        public string? link { get; set; }
        
        public string? NotifIndex { get; set; }

        public string? NotificationType { get; set; }

    }
}
