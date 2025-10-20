using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public class RequestExternalTravelRemove : BaseEntity
    {
        public int? DocumentId { get; set; }
        public int? EmployeeId { get; set; }
        public int? TransportId { get; set; }
    
    }
}
