using MediatR;
using System.Reflection;
using System.Runtime.CompilerServices;
using tas.Domain.Common;

namespace tas.Application.Features.EmployeeFeature.SearchEmployeeAccommodation
{
    public sealed record SearchEmployeeAccommodationRequest(EmployeeAccommodationRequest model) : BasePagenationRequest, IRequest<SearchEmployeeAccommodationResponse>;
}

public record EmployeeAccommodationRequest(
        string? Id,
        string? Lastname,
        string? Firstname,
        string? RoomNumber,
        int? Departmentid,
        int? LocationId,
        int? CostCodeId,
        string? NRN,
        int? RosterId,
        int? EmployerId,
        int? PeopleTypeId,
        int? PositionId,
        int? FlightGroupMasterId,
        string? SAPID,
        int? CampId,
        int? RoomTypeId,
        string? Mobile, 
        int? HasRoom,
        int? FutureBooking,
        int? Active
    );




