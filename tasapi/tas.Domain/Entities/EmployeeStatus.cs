using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;
using tas.Domain.CustomModel;

namespace tas.Domain.Entities
{
    public class EmployeeStatus : BaseEntity
    {
        public int? EmployeeId { get; set; }
        public DateTime? EventDate { get; set; }
        public int? RosterMasterId { get; set; }
        public int? DepId { get; set; }
        public int? PositionId { get; set; }
        public int? CostCodeId { get; set; }
        public int? EmployerId { get; set; }
        public int? ShiftId { get; set; }
        public int? BedId { get; set; }
        public int? RoomId { get; set; }
        public string? ChangeRoute { get; set; }
    }
}
