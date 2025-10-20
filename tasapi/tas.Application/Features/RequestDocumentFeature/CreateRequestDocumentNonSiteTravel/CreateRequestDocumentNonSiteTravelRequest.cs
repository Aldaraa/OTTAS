using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentNonSiteTravel
{

    public sealed record CreateRequestDocumentNonSiteTravelRequest(
        CreateRequestDocumentNonSiteTravel travelData,
        List<CreateRequestDocumentNonSiteTravelFlight> flightData,
        List<CreateRequestDocumentNonSiteTravelAccommodation> AccommodationData,
        List<CreateRequestDocumentNonSiteTravelAttachment> Files,
        CreateRequestDocumentNonSiteTravelRequestInfo RequestInfo


        ) : IRequest<int>;

    public sealed record CreateRequestDocumentNonSiteTravel
    (
        string? Description,
        int EmployeeId,
        string Action,
        int? AssignedEmployeeId,
        int NextGroupId,
        int RequestTravelPurposeId,
        decimal? HigestCost
    );

    public sealed record CreateRequestDocumentNonSiteTravelFlight
    (
        DateTime TravelDate,
        string? FavorTime,
        int ETD,
        int DepartLocationId,
        int ArriveLocationId,
        string? Comment

    );

    public sealed record CreateRequestDocumentNonSiteTravelAccommodation
    (
        DateTime FirstNight,
        DateTime LastNight,
        string? City,
        string? Hotel,
        string? HotelLocation,
        string? PaymentCondition,
        string? Comment,
        int? EarlyCheckIn,
        int? LateCheckOut

    );

    public sealed record CreateRequestDocumentNonSiteTravelAttachment
    (
        int  FileAddressId,
        string? Comment,
        int? IncludeEmail

    );

    public sealed record CreateRequestDocumentNonSiteTravelRequestInfo
    (
        string? Comment


    );


}
