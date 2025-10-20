using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTicketConfigFeature.ExtractOptioRequestNonSiteTicket
{
    public sealed record ExtractOptioRequestNonSiteTicketResponse
    {
        
        public string? BookIngType { get; set; }

        public decimal? TotalMinute { get; set; }
        public List<ExtractOptionRequestNonSiteTicketExtractedData> FlightData { get; set; }

        public List<ExtractOptionRequestNonSiteTicketExtractedDataTicketRule?> TicketRules { get; set; }
    }

    public sealed record ExtractOptionRequestNonSiteTicketExtractedDataTicketRule
    {
        public string AirlineCode { get; set; }

        public string? TicketCondition { get; set; }
        public decimal RefundCost { get; set; }
        public decimal Changes { get; set; }
        public decimal NoShowCost { get; set; }
        public string Baggage { get; set; }

        public string? LuggageAllowance { get; set; }
        public string? CarryOnAllowance { get; set; }



    }

    public sealed record ExtractOptionRequestNonSiteTicketExtractedData
    {
        public int? OptionNumber { get; set; }
        public string? AirlineCode { get; set; }


        public string? AirlineName { get; set; }
        public int? TransportNumber { get; set; }
        public string ClassOfSeat { get; set; }
        public DateTime? TransportDate { get; set; }
        public int? WeekNum { get; set; }

        public string? DayName { get; set; }
        public string? FromAirportCode { get; set; }
        public string? FromAirportCountry { get; set; }

        public string? FromAirportName { get; set; }

        public string? ToAirportCode { get; set; }

        public string? ToAirportCountry { get; set; }

        public string? ToAirportName { get; set; }





        public string? TicketStatus { get; set; }
        public string? ETD { get; set; }
        public string? ETA { get; set; }
        public DateTime? ArrivalDate { get; set; }

        public DateTime? DepartureDate { get; set; }
        public string? SeatType { get; set; }
        public int? TravelDurationMinutes { get; set; }

        public string? DepartureTimeZone { get; set; }
        public string? ArrivalTimeZone { get; set; }
    }
}
