using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public  class RequestSiteTravelReschedule : BaseEntity
    {
        public int DocumentId { get; set; }

        public int EmployeeId { get; set; }

        public int ExistingScheduleId { get; set; }

        public int ReScheduleId { get; set; }

        public int? RoomId { get; set; }

        public int ShiftId { get; set; }

        public string? Reason { get; set; }

        public string? ExistingScheduleIdDescr { get; set; }

        public string? ReScheduleIdDescr { get; set; }


        public int? ExistingScheduleNoShow { get; set; }

        public int? ReScheduleGoShow { get; set; }
    }
}
