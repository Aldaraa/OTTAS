
using tas.Domain.Common;

namespace tas.Application.Features.RoomFeature.DateProfileRoom
{


    public sealed record DateProfileRoomResponse : BasePaginationResponse<DateProfileRoomResponseDateEmployee>
    {

    }



    public sealed record DateProfileRoomResponseDateEmployee
    {
        public int Id { get; set; }
        public string? FullName { get; set; }

        public int? EmployeeId { get; set; }

        public bool? RoomOwner { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }


        public int? SAPID { get; set; }
        public string? Lastname { get; set; }

        public string? Firstname { get; set; }

        public string? PeopleTypeCode { get; set; }

        public int? Gender { get; set; }
        public int? HotelCheck { get; set; }

        public string? EmployerName { get; set; }

        public string? DepartmentName { get; set; }

        public int? DepartmentId { get; set; }

        public string? ShiftCode { get; set; }






        public List<DateProfileRoomResponseDateEmployeeBookingHistory> History { get; set; }


    }


    public sealed record DateProfileRoomResponseDateEmployeeBookingHistory
    {
        public int? RoomId { get; set; }

        public string? RoomNumber { get; set; }


        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

    }

    





}