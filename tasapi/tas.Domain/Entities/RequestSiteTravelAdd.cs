using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestSiteTravelAdd : BaseEntity
    {
        public int DocumentId { get; set; }

        public int EmployeeId { get; set; }

        public int inScheduleId { get; set; }

        public int outScheduleId { get; set;  }

        public int? RoomId { get; set;  }

        public int ShiftId { get; set; }
        
        public int DepartmentId { get; set;  }

        public int PositionId { get; set;  }

        public int CostCodeId { get; set;  }

        public int EmployerId { get; set;  }

        public string? Reason { get; set; }


        public string? inScheduleIdDescr { get; set; }

        public string? outScheduleIdDescr { get; set; }

        public int? inScheduleGoShow { get; set; }

        public int? outScheduleGoShow { get; set; }


    }
}
