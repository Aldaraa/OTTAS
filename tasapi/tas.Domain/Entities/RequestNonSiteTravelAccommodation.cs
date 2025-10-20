using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestNonSiteTravelAccommodation : BaseEntity
    {
        public int DocumentId { get; set; }

        public DateTime? FirstNight { get; set; }

        public DateTime? LastNight { get; set; }

        public string? City { get; set; }

        public string? Hotel { get; set; }

        public string? HotelLocation { get; set; }


        public string? PaymentCondition { get; set; }

        public string? Comment { get; set; }

        public int?  EarlyCheckIn {get ;set;}
        public int? LateCheckOut { get; set; }

        public decimal? DayCost { get; set; }
        public decimal? EarlyCheckInCost { get; set; }
        public decimal? LateCheckOutCost { get; set; }

        public decimal? AddCost { get;set;  }


    }
}
