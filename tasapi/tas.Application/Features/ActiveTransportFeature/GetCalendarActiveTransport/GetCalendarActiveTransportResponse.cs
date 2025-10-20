using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.ActiveTransportFeature.GetCalendarActiveTransport
{

    public sealed record GetCalendarActiveTransportResponse 
    {
        public int? Id { get; set; }

        public string? Code { get; set; }
        
        
        public string? Description { get; set; }

        public DateTime? EventDate { get; set; }

        public string? ETA { get; set; }

        public string? ETD { get; set; }

        public int? Seats { get; set; }

        public int? Booking { get; set; }

        public string? Direction { get; set; }

        public string? TransportMode { get; set; }

        public string? Carrier { get; set; }

        public string? FromLocationCode { get; set; }

        public string? ToLocationCode { get; set; }

        public string? TimeGroup { get; set; }





    }




}
