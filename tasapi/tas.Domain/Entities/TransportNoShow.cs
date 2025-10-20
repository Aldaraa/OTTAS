using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class TransportNoShow : BaseEntity
    {
        public int? EmployeeId { get; set; }
        public DateTime? EventDate { get; set; }
        public DateTime? EventDateTime { get; set; }
        public string? Direction { get; set; }

        public string? Reason { get; set; }

        public string? Description { get; set; }





    }
}
