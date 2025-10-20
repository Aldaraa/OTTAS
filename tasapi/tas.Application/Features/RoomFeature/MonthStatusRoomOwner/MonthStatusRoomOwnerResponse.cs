
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace tas.Application.Features.RoomFeature.MonthStatusRoomOwner
{ 
    public sealed record MonthStatusRoomOwnerResponse
    {
        public int? Id { get; set; }
        public int? SAPID { get; set; }

        public string? Lastname { get; set; }
        public string? Firstname { get; set; }

        public int? Gender { get; set; }

        public string? EmployerName { get; set; }

        public string? DepartmentName { get; set; }

        public int? DepartmentId { get; set; }



        public string? PositionName { get; set; }
        public string? PeopleTypeName { get; set; }

        public int? HotelCheck { get; set; }


        public DateTime? futureTransportDate { get; set; }

        public string? futureTransportScheduleDescription { get; set; }

        public DateTime? futureTransportOUTDate { get; set; }

        public string? futureTransportOUTScheduleDescription { get; set; }


    }


}