using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public class TransportAudit : BaseEntity
    { 
        public DateTime? EventDate { get; set; }
        public int? EmployeeId { get; set; }
        public int? DepartmentId { get; set; }
        
        public int? ScheduleId { get; set; }
        public string? Direction { get; set; }

        public int? OldScheduleId { get; set; }
        public string? OldDirection { get; set; }

        public string? UpdateSource { get; set; }


        public int? NoShow { get; set; }

        public DateTime? OldTransportDate { get; set; }



    }
}
