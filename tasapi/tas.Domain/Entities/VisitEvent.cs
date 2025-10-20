using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class VisitEvent : BaseEntity
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set;   }

        public DateTime EndDate { get; set; }

        public int HeadCount { get; set; }

        public int? InScheduleId { get; set; }

        public int? OutScheduleId { get; set; }



    }
}
