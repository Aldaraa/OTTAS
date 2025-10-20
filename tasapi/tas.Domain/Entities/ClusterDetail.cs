using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class ClusterDetail : BaseEntity
    {
        public int ActiveTransportId { get; set; }

        public int ClusterId { get; set; }

        public int? SeqNumber { get; set; }

    }
}
