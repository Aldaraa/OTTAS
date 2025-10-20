using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestGroupEmployee : BaseEntity
    {
        public int Id { get; set; }
        public int RequestGroupId { get; set; }

        public string? DisplayName { get; set; }

        public int? EmployeeId { get; set; }

        public int PrimaryContact { get; set; }

        public int OrderIndex { get; set; }

    }

}
