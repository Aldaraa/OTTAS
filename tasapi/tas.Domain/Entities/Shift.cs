using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public class Shift : BaseEntity
    {
        public int? ColorId { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public int OnSite { get; set; } = 0;

        public int isDefault { get; set; } = 0;

        public int? TransportGroup { get; set; } = 0;
    }
}
