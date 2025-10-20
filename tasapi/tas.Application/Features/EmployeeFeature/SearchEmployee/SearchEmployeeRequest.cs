using MediatR;
using System.Reflection;
using System.Runtime.CompilerServices;
using tas.Domain.Common;

namespace tas.Application.Features.EmployeeFeature.SearchEmployee
{
    public sealed record SearchEmployeeRequest(RequestModel model) : BasePagenationRequest, IRequest<SearchEmployeeResponse>;
}

public record RequestModel(
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
        int? PositionId,
        int? PeopleTypeId,
        int? FlightGroupMasterId,
        string? SAPID,
        int? CampId,
        int? RoomTypeId,
        string? Mobile, 
        int? HasRoom,
        int? FutureBooking,
        int? Active

    );




