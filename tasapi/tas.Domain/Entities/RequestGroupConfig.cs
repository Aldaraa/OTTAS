using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestGroupConfig : BaseEntity
    {
        public string? Document { get; set; }

        public int GroupId { get; set; }

        public int OrderIndex { get; set; }

        public string? RuleAction { get; set; }


    }
}
