using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.GetRequestDocumentProfileChange
{
    public sealed record GetRequestDocumentProfileChangeResponse
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

        public string? DocumentTag { get; set; }


        public GetRequestDocumentProfileChangeEmployee employee { get; set; }

        public List<GetRequestDocumentProfileChangeEmployeeChangeInfo>? ChangeInfo { get; set; }

    }

    public sealed record GetRequestDocumentProfileChangeEmployee
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string? MFirstname { get; set; }
        public string? MLastname { get; set; }
        public string? Mobile { get; set; }
        public string? PersonalMobile { get; set; }
        public string? Email { get; set; }
        public DateTime? Dob { get; set; }
        public int? Active { get; set; }
        public int? Gender { get; set; }
        public int? SAPID { get; set; }
        public string? PassportNumber { get; set; }
        public string? PassportName { get; set; }
        public DateTime? PassportExpiry { get; set; }
        public string? PassportImage { get; set; }
        public DateTime? CommenceDate { get; set; }
        public string? NRN { get; set; }
        public int? LoginEnabled { get; set; } = 0;
        public int? HotelCheck { get; set; } = 0;
        public int? NationalityId { get; set; }

        public string? NationalityName { get; set; }

        public int? SiteContactEmployeeId { get; set; }

        public string? SiteContactEmployeeFirstname { get; set; }

        public string? SiteContactEmployeeLastname { get; set; }

        public string? SiteContactEmployeeMobile { get; set; }


        public int? EmployerId { get; set; }

        public string? EmployerName { get; set; }

        public int? StateId { get; set; }

        public string? StateName { get; set; }

        public int? ShiftId { get; set; }

        public string? ShiftName { get; set; }

        public string? ContractNumber { get; set; }
        public string? ADAccount { get; set; }
        public int? CostCodeId { get; set; }

        public string? CostCodeName { get; set; }

        public int? DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public int? PositionId { get; set; }

        public string? PositionName { get; set; }
        public int? RosterId { get; set; }

        public string? RosterName { get; set; }
        public int? LocationId { get; set; }

        public string? LocationName { get; set; }

        public int? PeopleTypeId { get; set; }

        public string? PeopleTypeName { get; set; }
        public string? PickUpAddress { get; set; }
        public int? RoomId { get; set; }
        public int? FlightGroupMasterId { get; set; }

        public string? FlightGroupMasterName { get; set; }

        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactMobile { get; set; }

        public DateTime? CompletionDate { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string? RoomNumber { get; set; }

        public int? RoomTypeId { get; set; }

        public DateTime? NextRosterDate { get; set; }

        public List<GetRequestDocumentProfileChangeEmployeeStatusDate>? employeeStatusDates { get; set; }

        public List<GetRequestDocumentProfileChangeEmployeeTransport>? employeeTransports { get; set; }

        public List<GetRequestDocumentProfileChangeEmployeeInfoGroup>? GroupData { get; set; }


    }


    public sealed record GetRequestDocumentProfileChangeEmployeeChangeInfo
    {
        public string? FieldName { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

    }



    public sealed record GetRequestDocumentProfileChangeEmployeeStatusDate
    { 
        public DateTime? EventDate { get; set; }

        public string? ShiftCode { get; set; }

        public string? Direction { get; set; }

        public string? Color { get; set; }

        public string? Schedule { get; set; }
    }

    public sealed record GetRequestDocumentProfileChangeEmployeeTransport
    {
        public DateTime? EventDate { get; set; }

        public string? Direction { get; set; }

        public string? Description { get; set; }
    }

    public sealed record GetRequestDocumentProfileChangeEmployeeInfoGroup
    {
        public int? Id { get; set; }
        public int? GroupDetailId { get; set; }

        public int? GroupMasterId { get; set; }

    }





}
