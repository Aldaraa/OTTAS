using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;
using tas.Domain.CustomModel;

namespace tas.Domain.Entities
{
    [Audit]
    public class Room : BaseEntity
    {
        public string Number { get; set; }

        public int BedCount { get; set; }

        public int Private { get; set; }

        public int? RoomTypeId { get; set; }

        public int CampId { get; set; }

        public int VirtualRoom { get; set;  } = 0;

    }
}
