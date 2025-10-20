using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestDeMobilisation : BaseEntity
    {
        public int DocumentId { get; set; }

        public DateTime? CompletionDate { get; set; }

        public int? EmployerId { get; set; }

        public int? RequestDeMobilisationTypeId { get; set; }

        public string? Comment { get; set; }

    }
}
