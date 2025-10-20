using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestLocalHotel : BaseEntity
    {
        public string Description { get; set; }

        public decimal? DayCost { get; set; }
        public decimal? EarlyCheckInCost { get; set; }
        public decimal? LateCheckOutCost { get; set; }



    }
}
