using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ActiveTransportFeature.GetAllActiveTransport;
using tas.Domain.Common;

namespace tas.Application.Features.EmployeeFeature.SearchEmployeeAccommodation
{
    public sealed record SearchEmployeeAccommodationResponse : BasePaginationResponse<SearchEmployeeAccommodationSearchResult>
    {
        public List<int> NotFoundSAPIDs { get; set; }
    }



    public sealed class SearchEmployeeAccommodationSearchResult
    {

        public int Id { get; set; }
        public string? Lastname { get; set; }
        public string? Firstname { get; set; }
        public string? MFirstname { get; set; }
        public string? MLastname { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? EmployerName { get; set; }
        public int? StateId { get; set; }
        public string? StateName { get; set; }
        public int? ShiftId { get; set; }
        public string? ShiftName { get; set; }
        public string? CostCodeName { get; set; }
        public string? DepartmentName { get; set; }
        public string? PositionName { get; set; }
        public string? RosterName { get; set; }
        public string? LocationName { get; set; }
        public string? PeopleTypeName { get; set; }
        public string? RoomNumber { get; set; }

        public string? RoomTypeName { get; set; }
        public string? FlightGroupMasterName { get; set; }

        public int? Active { get; set; }
        public int? Gender { get; set; }

        public int?  SAPID { get; set; }

        public int? CostCodeId { get; set; }

        public int? FlightGroupMasterId { get; set; }

        public int? LocationId { get; set; }

        public int? PeopleTypeId { get; set; }

        public int? RosterId { get; set; }

        public int? Departmentid { get; set; }

        public string? NRN { get; set; }

        public int? RoomId { get; set; }

        public int? EmployerId { get; set; }

        public int? HasFutureTransport { get; set; }

        public int? HasFutureRoomBooking { get; set; }

        public int? RoomTypeId { get; set; }

        public bool? TodayOnsite { get; set; }

        public int? PositionId { get; set; }





    }
}
