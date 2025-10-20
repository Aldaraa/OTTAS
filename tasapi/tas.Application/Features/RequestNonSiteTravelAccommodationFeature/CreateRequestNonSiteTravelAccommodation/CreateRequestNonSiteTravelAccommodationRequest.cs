using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTravelAccommodationFeature.CreateRequestNonSiteTravelAccommodation
{
    public sealed record CreateRequestNonSiteTravelAccommodationRequest(
        DateTime? FirstNight,
        DateTime? LastNight,
        string? City,
        string? Hotel,
        string? HotelLocation,
        string? PaymentCondition,
        string? Comment,
        int DocumentId,
        int? EarlyCheckIn,
        int? LateCheckOut,
        decimal? DayCost,
        decimal? EarlyCheckInCost,
        decimal? LateCheckOutCost,
        decimal? AddCost

        ) : IRequest;
}
