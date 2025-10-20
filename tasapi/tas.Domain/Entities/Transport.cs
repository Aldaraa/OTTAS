using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;
using tas.Domain.CustomModel;

namespace tas.Domain.Entities
{

    public sealed class Transport : BaseEntity
    {
        public int? EmployeeId { get; set; }
        public DateTime? EventDate { get; set; }
        public DateTime? EventDateTime { get; set; }
        public int? CostCodeId { get; set; }
        public int? DepId { get; set; }
        public string Direction { get; set; }
        public int? PositionId { get; set; }
        public int? EmployerId { get; set; }

        public int? ScheduleId { get; set; }

        public string? Status { get; set; }
        public int ActiveTransportId { get; set; }

        public int? SeatBlock { get; set; } = 0;

        public string? ChangeRoute { get; set; }

        public int? NoShow { get; set; }

        public int? GoShow { get; set; }



        // public int Active { get; set; }

    }
}
