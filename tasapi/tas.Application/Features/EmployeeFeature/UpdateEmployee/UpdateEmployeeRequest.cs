using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.LocationFeature.GetAllLocation;

namespace tas.Application.Features.EmployeeFeature.UpdateEmployee
{
    public sealed record UpdateEmployeeRequest(
     int Id,
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
     string? PassportImage,
     DateTime? CommenceDate,
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
     int? FlightGroupMasterId,
     string? EmergencyContactName,
     string? EmergencyContactMobile,
     DateTime? CompletionDate,
     IFormFile? PassportRawImage,
     int? FrequentFlyer,
     int? CreateRequest,
     int? CampId,
     int? RoomTypeId,
     string? Hometown 

    ) : IRequest;





}
