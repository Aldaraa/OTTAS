using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.CreateEmployeeRequest
{
    public sealed record CreateEmployeeRequestRequest(
        string Lastname,
        string Firstname,
        string? MFirstname,
        string? MLastname,
        string? Mobile,
        string? PersonalMobile,
        string? Email,
        DateTime? Dob,
        int? Active,
        int? Gender,
        int? SAPID,
        string? PassportNumber,
        string? PassportName,
        DateTime? PassportExpiry,
       // string PassportImage,
        DateTime? CommenceDate,
        DateTime? CompletionDate,
        string? NRN,
        int? LoginEnabled,
        int? HotelCheck,
        int? NationalityId,
        int? SiteContactEmployeeId,
        int? EmployerId,
        int? StateId,
        int? ShiftId,
        string? ContractNumber,
        string? ADAccount,
        int? CostCodeId,
        int? DepartmentId,
        int? PositionId,
        int? RosterId,
        int? LocationId,
        int? PeopleTypeId,
        string? PickUpAddress,
        int? RoomId,
        int? FlightGroupMasterId,
        string? EmergencyContactName,
        string? EmergencyContactMobile,
        int? FrequentFlyer
        ) : IRequest<int>;
}
