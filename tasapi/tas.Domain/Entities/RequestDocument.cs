using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestDocument : BaseEntity
    {
        public string? DocumentType { get; set; }

        public string? Description { get; set; }

        public string? CurrentAction { get; set; }

        public int? EmployeeId { get; set; }

        public int? AssignedEmployeeId { get; set; }

        public int? AssignedRouteConfigId { get; set; }

        public string? DocumentTag  { get; set; }
         public DateTime? DaysAwayDate { get; set; }

        public string? UpdatedInfo { get; set; }

        public string? MailDescription { get; set; }
        public int? DeclinedUserId { get; set; }
        public int? CompletedUserId { get; set; }
        public DateTime? DeclinedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

    }
}
