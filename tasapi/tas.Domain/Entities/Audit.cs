using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;
using tas.Domain.Enums;

namespace tas.Domain.Entities
{
    public sealed  class Audit : BaseEntity
    {
        public int? UserId { get; set; }
        public string? UserName { get; set; }

        public string? Type { get; set; }
        public string? TableName { get; set; }
        public DateTime? DateTime { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? AffectedColumns { get; set; }
        public string? PrimaryKey { get; set; }


    }


}
