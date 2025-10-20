using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public class RequestTransport : BaseEntity
    {
        public int? EmployeeId { get; set; }
        public DateTime? EventDate { get; set; }
        public DateTime? EventDateTime { get; set; }

        public string Direction { get; set; }
      
        public int? ScheduleId { get; set; }

        public int? ActiveTransportId { get; set; }

        public int? DocumentId { get; set; } = 0;

    }
}
