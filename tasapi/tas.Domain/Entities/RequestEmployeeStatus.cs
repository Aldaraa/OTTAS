using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestEmployeeStatus : BaseEntity
    {
        public int? EmployeeId { get; set; }
        public DateTime? EventDate { get; set; }
        public int? RosterMasterId { get; set; }
        public int? ShiftId { get; set; }
        public int? BedId { get; set; }
        public int? RoomId { get; set; }

        public int? DocumentId { get; set; }
    }
}
