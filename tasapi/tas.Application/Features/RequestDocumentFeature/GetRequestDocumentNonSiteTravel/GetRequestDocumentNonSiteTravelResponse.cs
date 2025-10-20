using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentFeature.GetRequestDocumentNonSiteTravel
{
    public sealed record GetRequestDocumentNonSiteTravelResponse
    {
        public int? DaysAway { get; set; }
        public int Id { get; set; }

        public string? CurrentStatus { get; set; }
        public string? DocumentType { get; set; }

        public string? RequesterFullName { get; set; }


        public string? RequesterMobile { get; set; }

        public string? RequesterMail { get; set; }

        public int? RequestUserId { get; set; }

        public string? AssignedEmployeeFullName { get; set; }

        public DateTime? RequestedDate { get; set; }

        public string? EmployeeFullName { get; set; }

        public int? EmployeeId { get; set; }

        public string? UpdatedInfo { get; set; }

        public int? AssignedEmployeeId { get; set; }

        public int? AssignedRouteConfigId { get; set; }

        public int? DelegateEmployeeId { get; set; }

        public int? IssuedoptionId { get; set; }

        

        public List<GetRequestDocumentNonSiteTravelAccommodation> Accommodations { get; set; }

        public GetRequestNonSiteTravelFlightInfo FlightInfo { get; set; }



    }

    public record GetRequestNonSiteTravelFlightInfo
    {
        public int? travelId { get; set; }
        public decimal? Cost { get; set; }

        public decimal? Cost2 { get; set; }

        public decimal? HighestCost { get; set; }


        public int? RequestTravelAgentId { get; set; }

        public string? RequestTravelAgentSureName { get; set; }

        public int? RequestTravelPurposeId { get; set; }

        public string? RequestTravelPurposeDescription { get; set; }



        public List<GetRequestNonSiteTravelFlight> FlightData { get; set; }
    }


    public sealed record GetRequestNonSiteTravelFlight
    {

        public int Id { get; set; }
        public DateTime? TravelDate { get; set; }
        public string? FavorTime { get; set; }
        public int? ETD { get; set; }
        public int? DepartLocationId { get; set; }
        public string? DepartLocationName { get; set; }
        public int? ArriveLocationId { get; set; }
        public string? ArriveLocationName { get; set; }
        public string? Comment { get; set; }
        public int DocumentId { get; set; }
    }



    public sealed record GetRequestDocumentNonSiteTravelAccommodation
    {

        public int Id { get; set; }

        public string? Hotel { get; set; }

        public string? HotelLocation { get; set; }

        public string? PaymentCondition { get; set; }

        public string? City { get; set; }

        public DateTime? FirstNight { get; set; }
        public DateTime? LastNight { get; set; }

        public string? Comment { get; set; }


        public int DocumentId { get; set; }

        public int? EarlyCheckIn { get; set; }
        public int? LateCheckOut { get; set; }

        public int? NightOfNumbers { get; set; }



        public decimal? DayCost { get; set; }
        public decimal? EarlyCheckInCost { get; set; }
        public decimal? LateCheckOutCost { get; set; }

        public decimal? AddCost { get; set; }




    }
}
