using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public  class RequestExternalTravelReschedule : BaseEntity
    {
        public int? DocumentId { get; set; }
        public int? EmployeeId { get; set; }
        public int? ScheduleId { get; set; }

        public int? oldTransportId { get; set; }
    }
}
