using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTravelFlightFeature.CreateRequestNonSiteTravelFlight
{
    public sealed record CreateRequestNonSiteTravelFlightRequest(
        DateTime TravelDate,
        string FavorTime,
        int? ETD,
        int DepartLocationId,
        int ArriveLocationId,
        string? Comment,
        int DocumentId

        ) : IRequest;
}
