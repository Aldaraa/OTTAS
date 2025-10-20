using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public  class TransportScheduleBusstop : BaseEntity
    {
        public string? Description { get; set; }

        public string? ETD { get; set; }

        public int? ScheduleId { get; set; }

    }
}
