using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestDocumentAttachment : BaseEntity
    {
        public int DocumentId { get; set; }

        public string? FileAddress { get; set; }

        public string? Description { get; set; }

        public int? IncludeEmail { get; set; }
    }
}
