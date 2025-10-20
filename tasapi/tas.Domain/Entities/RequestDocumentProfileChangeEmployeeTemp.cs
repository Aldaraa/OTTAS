using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public class RequestDocumentProfileChangeEmployeeTemp : BaseEntity
    {
        public int? DocumentId { get; set; }
        public int? CostCodeId { get; set; }
        public int? DepartmentId { get; set; }
        public int? EmployerId { get; set; }
        public int? PositionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Permanent { get; set; }
    }
}
