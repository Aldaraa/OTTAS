using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{

    public class RoomAudit : BaseEntity
    {
        public DateTime? EventDate { get; set; }
        public int? EmployeeId { get; set; }
        public int? DepartmentId { get; set; }
        public int? ShiftId { get; set; }
        public int? RoomId { get; set; }
        public int? BedId { get; set; }


        public int? OldRoomId { get; set; }
        public int? OldBedId { get; set; }

        public string? UpdateSource { get; set; }

    }
}
