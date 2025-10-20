using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestAirportFeature.GetAllRequestAirport
{
    public sealed record GetAllRequestAirportResponse
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        public string? Description { get; set; }

        public string? Country { get; set; }
        public int Active { get; set; }

        public int? OrderIndex  { get; set; }



        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
