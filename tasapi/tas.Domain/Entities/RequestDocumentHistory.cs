using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestDocumentHistory : BaseEntity
    {
        public int DocumentId { get; set; }
        public string? Comment { get; set; }

        public string CurrentAction { get; set; }

        public int? ActionEmployeeId { get; set; }

        public int? AssignedGroupId { get; set; }

    }
}
