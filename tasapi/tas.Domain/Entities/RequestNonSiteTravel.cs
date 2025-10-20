using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestNonSiteTravel : BaseEntity
    {
        public int DocumentId { get; set; }

        public int? RequestTravelAgentId { get; set; }
        public int? RequestTravelPurposeId { get; set; }

        public decimal? Cost { get; set; }

        public decimal? Cost2 { get; set; } 

        public decimal? HigestCost { get; set; }


        public string? RequestTravelAgentSureName { get; set; }
    }
}
