using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTravelAccommodationFeature.UpdateRequestNonSiteTravelAccommodation
{
    public sealed record UpdateRequestNonSiteTravelAccommodationRequest(
        int Id,
        DateTime? FirstNight,
        DateTime? LastNight,
        string? City,
        string? Hotel,
        string? HotelLocation,
        string? PaymentCondition,
        string? Comment,
        int DocumentId,
        int? LateCheckOut,
        int? EarlyCheckIn,
        decimal? DayCost,
        decimal? EarlyCheckInCost,
        decimal? LateCheckOutCost,
        decimal? AddCost

        ) : IRequest;
}
