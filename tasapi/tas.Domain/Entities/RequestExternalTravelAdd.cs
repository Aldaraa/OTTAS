using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public  class RequestExternalTravelAdd : BaseEntity
    {
        public int DocumentId { get; set; }

        public int EmployeeId { get; set; }

        public int FirstScheduleId { get; set; }

        public int? LastScheduleId { get; set; }

        public int DepartmentId { get; set; }

        public int PositionId { get; set; }

        public int CostCodeId { get; set; }

        public int EmployerId { get; set; }


    }
}
