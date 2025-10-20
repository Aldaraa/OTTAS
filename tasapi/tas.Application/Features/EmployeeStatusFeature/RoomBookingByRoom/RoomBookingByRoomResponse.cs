using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RoomFeature.MonthStatusRoom;
using tas.Domain.Common;

namespace tas.Application.Features.EmployeeStatusFeature.RoomBookingByRoom
{

    public sealed record RoomBookingByRoomResponse : BasePaginationResponse<RoomBookingByRoomResponseData>
    {

    }


    public sealed record RoomBookingByRoomResponseData
    {
        public int Id { get; set; }

        public int?  EmployeeId { get; set; }
        public string? FullName { get; set; }

        public string? Camp { get; set; }

        public string? RoomType { get; set; }
        public string? RoomNumber { get; set; }

        public int? VirtualRoom { get; set; }

        public int? RoomId { get; set; }

        public string? DateIn { get; set; }

        public string? LastNight { get; set; }

        public int? Days { get; set; }


        public int? CostCodeId { get; set; }

        public string? CostCodeDescription { get; set; }


        public int? SAPID { get; set; }
        public string? Lastname { get; set; }

        public string? Firstname { get; set; }

        public string? PeopleTypeCode { get; set; }

        public int? Gender { get; set; }
        public int? HotelCheck { get; set; }

        public string? EmployerName { get; set; }

        public string? DepartmentName { get; set; }

        public int? DepartmentId { get; set; }


        public string? PersonalMobile { get; set; }

        public bool? RoomOwner { get; set; }
    }
}
