using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestNonSiteTravelFlight : BaseEntity
    {
        public int DocumentId { get; set; }

        public DateTime? TravelDate { get; set; }

        public string? FavorTime { get; set; }

        public int? ETD { get; set; }

        public int? DepartLocationId { get; set; }

        public int? ArriveLocationId { get; set; }

        public string? Comment { get; set; }

    }
}
