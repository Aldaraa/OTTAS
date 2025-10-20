using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestGroup : BaseEntity
    {
        public string Description { get; set; }

        public int ReadOnly { get; set; }

        public string? GroupTag { get; set; }

    }
}
