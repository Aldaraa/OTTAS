using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class FlightGroupDetail : BaseEntity
    {
        public int FlightGroupMasterId { get; set; }

        public int ShiftId { get; set; }

        public int? ClusterId { get; set; }

        public string? Direction { get; set; }

        public string? DayNum { get; set; }

        public int? SeqNumber { get; set; }


    }
}
