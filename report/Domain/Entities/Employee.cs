using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public sealed class Employee : BaseEntity
    {
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
        public int? LoginEnabled { get; set; }
        public int? HotelCheck { get; set; }
        public int? NationalityId { get; set; }
        public int? SiteContactEmployeeId { get; set; }
        public int? EmployerId { get; set; }
        public int? StateId { get; set; }
        public string? ContractNumber { get; set; }
        public string? ADAccount { get; set; }
        public int? CostCodeId { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public int? RosterId { get; set; }
        public int? LocationId { get; set; }
        public int? PeopleTypeId { get; set; }
        public string? PickUpAddress { get; set; }
        public int? RoomId { get; set; }
        public int? FlightGroupMasterId { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactMobile { get; set; }
        public int? Shiftid { get; set; }
        public int? FrequentFlyer { get; set; }
        public DateTime? CompletionDate { get; set; }

        public int? CreateRequest { get; set; }
        public int? CampId { get; set; }
        public int? RoomTypeId { get; set; }

        public string? Hometown { get; set; }

    }
}
