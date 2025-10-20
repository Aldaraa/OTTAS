
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using tas.Domain.Common;

namespace tas.Application.Features.RoomFeature.MonthStatusRoom
{ 
    public sealed record MonthStatusRoomResponse : BasePaginationResponse<MonthStatusRoomEmployees>
    {

    }

    


    public sealed record MonthStatusRoomEmployees 
    {
        public int Id { get; set; }

        public int? SAPID { get; set; }
        public string? Lastname { get; set; }

        public string? Firstname { get; set; }

        public bool? RoomOwner { get; set; }

        public string? PeopleTypeCode { get; set; }

        public int? Gender { get; set; }
        public int? HotelCheck { get; set; }

        public string? EmployerName { get; set; }

        public string? DepartmentName { get; set; }

        public int? DepartmentId { get; set; }

        public string? PositionName { get; set; }



        public int? RoomId { get; set; }

        public string? RoomNumber { get; set; }






        [NotMapped]
        public Dictionary<string, MonthStatusRoomResponseDateInfo> OccupancyData { get; set; }



        [NotMapped]
        public Dictionary<string, MonthStatusAnotherRoomResponseDateInfo> AnotherRoomData { get; set; }
    }


    public sealed record MonthStatusAnotherRoomResponseDateInfo
    {
        public int? RoomId { get; set; }

        public string? RoomNumber { get; set; }


    }


    public sealed record MonthStatusRoomResponseDateInfo
    {
        public string? ShiftCode { get; set; }

        public string? ShiftDescription { get; set; }


        public string? ShiftColor { get; set; }

        public string? transportDirection { get; set; }


        public string? transporScheduleCode { get; set; }


    }


    //public sealed record MonthStatusRoomOwnerInfo
    //{
    //    public int? Id { get; set; }
    //    public int? SAPID { get; set; }

    //    public string? Lastname { get; set; }
    //    public string? Firstname { get; set; }

    //    public int? Gender { get; set; }

    //    public string? EmployerName { get; set; }

    //    public string? DepartmentName { get; set; }

    //    public string? PositionName { get; set; }
    //    public string? PeopleTypeName { get; set; }

    //    public int? HotelCheck { get; set; }


    //    public DateTime? futureTransportDate {get; set;}

    //    public string? futureTransportScheduleDescription { get; set; }

    //    public DateTime? futureTransportOUTDate { get; set; }

    //    public string? futureTransportOUTScheduleDescription { get; set; }

    //}

}