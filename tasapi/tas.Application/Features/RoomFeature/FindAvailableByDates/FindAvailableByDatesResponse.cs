
using tas.Domain.Common;

namespace tas.Application.Features.RoomFeature.FindAvailableByDates
{ 
    public sealed record FindAvailableByDatesResponse  
    {

        public string? roomNumber { get; set; }
        public int? RoomId { get; set; }

        public int? BedCount { get; set; }

        public int? VirtulRoom { get; set; }

        public int? Employees { get; set; }

        public int? RoomOwners { get; set; }

        public DateTime? OwnerInDate { get; set; }


    }






    public sealed record FindAvailableByDatesOccupancyCurrentInfo
    {
        public int Id { get; set; }
        public int? SAPID { get; set; }

        public string? Lastname { get; set; }
        public string? Firstname { get; set; }

        public int? Gender { get; set; }

        public string? EmployerName { get; set; }

        public string? DepartmentName { get; set; }

        public string? PeopleTypeName { get; set; }

        public int? HotelCheck { get; set; }

    }

    public sealed record FindAvailableByDatesOccupancyHistoryInfo
    {
        public int Id { get; set; }
        public int? SAPID { get; set; }

        public string? Lastname { get; set; }
        public string? Firstname { get; set; }


        public int? Gender { get; set; }

        public string? EmployerName { get; set; }

        public string? DepartmentName { get; set; }

        public string? PeopleTypeName { get; set; }

        public int? HotelCheck { get; set; }



    }


}