using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.GetEmployee
{
    public sealed record GetEmployeeResponse
    {
        public int Id { get; set; }
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

        public DateTime? CompletionDate { get; set; }

        public string? NationalityName { get; set; }

        public int? SiteContactEmployeeId { get; set; }

        public string? SiteContactEmployeeFirstname { get; set; }

        public string? SiteContactEmployeeLastname { get; set; }

        public string? SiteContactEmployeeMobile { get; set; }


        public int? EmployerId { get; set; }

        public string? EmployerName { get; set; }

        public int? EmployerCurrentStatus { get; set; }

        public int? StateId { get; set; }

        public string? StateName { get; set; }

        public int? ShiftId { get; set; }

        public string? ShiftName { get; set; }

        public string? ContractNumber { get; set; }
        public string? ADAccount { get; set; }
        public int? CostCodeId { get; set; }

        public string? CostCodeName { get; set; }

        public string? CostCodeDescription { get; set; }
        public string? CostCodeNumber { get; set; }

        public int? CostCodeCurrentStatus { get; set; }

        public int? DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public int? DepartmentCurrentStatus { get; set; }



        public int? PositionId { get; set; }

        public string? PositionName { get; set; }

        public int? PositionCurrentStatus { get; set; }

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

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string? RoomNumber { get; set; }
        public string? CurrentRoomNumber { get; set; }

        public int? CurrentRoomId { get; set; }

        public int? OnSiteStatus { get; set; }
 

        public DateTime? NextRosterDate { get; set; }

        public int? FrequentFlyer { get; set; }

        public int? CreateRequest { get; set; }

        public int? CampId { get; set; }

        public int? RoomTypeId { get; set; }

        public string? CampRoomType { get; set; }

        public string? Hometown { get; set; }

        public DateTime? RosterExecutedDate { get; set; }
        public int? RosterExecuteMonthDuration { get; set; }
        public DateTime? RosterExecuteLastDate { get; set; }



        public List<EmployeeStatusDate>? employeeStatusDates { get; set; }

        public List<EmployeeTransport>? employeeTransports { get; set; }

        public List<EmployeeInfoGroup>? GroupData { get; set; }

     //   public List<GetProfileEmployeeHistory> EmployeeHistories { get; set; }
    }

    public sealed record EmployeeStatusDate
    { 
        public DateTime? EventDate { get; set; }

        public string? ShiftCode { get; set; }

        public string? Direction { get; set; }

        public string? Color { get; set; }

        public string? Schedule { get; set; }

        public string? TransportMode { get; set; }
    }

    public sealed record EmployeeTransport
    {
        public DateTime? EventDate { get; set; }

        public DateTime? EventDateTime { get; set; }



        public string? Direction { get; set; }

        public string? Description { get; set; }

        public string? TransportMode { get; set; }

        public string? TransportCode { get; set; }
    }

    public sealed record EmployeeInfoGroup
    {
        public int? Id { get; set; }
        public int? GroupDetailId { get; set; }

        public int? GroupMasterId { get; set; }

    }

    //public sealed record GetProfileEmployeeHistory
    //{ 
    //    public int? Id { get; set; }
        
    //    public DateTime? EventDate { get; set; }

    //    public string? Comment { get; set; }

    //    public string TerminationTypeName { get; set; }

    //    public string? Action { get; set; }

    //    public int EmployeeId { get; set; }
    //}



}
