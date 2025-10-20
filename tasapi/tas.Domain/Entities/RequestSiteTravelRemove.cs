using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public class RequestSiteTravelRemove : BaseEntity
    {
        public int DocumentId { get; set; }

        public int EmployeeId { get; set; }

        public int FirstScheduleId { get; set; }

        public int LastScheduleId { get; set; }

        public int? RoomId { get; set; }

        public int ShiftId { get; set; }

        public string? Reason { get; set; }

        public int? CostCodeId { get; set; }


        public string? FirstScheduleIdDescr { get; set; }

        public string? LastScheduleIdDescr { get; set; }

        public int? FirstScheduleNoShow { get; set; }

        public int? LastScheduleNoShow { get; set; }
    }
}
