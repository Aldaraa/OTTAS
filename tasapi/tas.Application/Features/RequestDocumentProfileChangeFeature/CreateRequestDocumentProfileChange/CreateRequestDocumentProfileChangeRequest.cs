using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.CreateRequestDocumentProfileChange
{

    public sealed record CreateRequestDocumentProfileChangeRequest(
        CreateRequestDocumentProfileChangeEmployee Employee,
        CreateRequestDocumentProfileChangeData ChangeRequestData 
        ) : IRequest<int>;

    public sealed record CreateRequestDocumentProfileChangeData
    (
        string? Comment,
        int? AssignedEmployeeId,
        string Action,
        int NextGroupId


    );
    public sealed record CreateRequestDocumentProfileChangeEmployee
    (
         int EmployeeId,
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
         int? RoomId,
         int? FlightGroupMasterId,
         string? EmergencyContactName,
         string? EmergencyContactMobile,
         int? PassportRawImageFileAddressId
    );





}
