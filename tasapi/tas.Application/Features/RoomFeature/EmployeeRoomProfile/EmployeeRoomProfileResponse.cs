
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.EmployeeRoomProfile
{ 
    public sealed record EmployeeRoomProfileResponse
    {
        public int Id { get; set; }
        public int? SAPID { get; set; }

        public string? Lastname { get; set; }
        public string? Firstname { get; set; }

        public int? Gender { get; set; }

        public string? EmployerName { get; set; }

        public string? DepartmentName { get; set; }

        public string? PositionName { get; set; }

        public string? PeopleTypeName { get; set; }

        public int? HotelCheck { get; set; }

        public string? RoomNumber { get; set; }


        public List<EmployeeRoomProfileRoomHistory> employeeRoomProfileRoomHistories { get; set; }



    }

    public sealed record EmployeeRoomProfileRoomHistory
    {
        public int RoomId { get; set; }

        public string? RoomNumber { get; set; }

        public string? Camp { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }


    }
}