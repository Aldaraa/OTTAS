using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestLocalHotelFeature.GetAllRequestLocalHotel
{
    public sealed record GetAllRequestLocalHotelResponse
    {
        public int Id { get; set; }


        public string? Description { get; set; }
        public int Active { get; set; }

        public decimal? DayCost { get; set; }
        public decimal? EarlyCheckInCost { get; set; }
        public decimal? LateCheckOutCost { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
