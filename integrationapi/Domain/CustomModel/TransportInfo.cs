using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CustomModel
{
    public sealed class TransportInfo
    {
        public DateTime? EventDate { get; set; }
        public string? Direction { get; set; }

        public string? ScheduleDescription { get; set; }

        public string? TransportMode { get; set; }

        public string? RoomNumber { get; set; }



    }
}
