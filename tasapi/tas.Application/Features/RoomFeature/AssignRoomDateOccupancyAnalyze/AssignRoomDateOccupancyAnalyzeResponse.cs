
namespace tas.Application.Features.RoomFeature.AssignRoomDateOccupancyAnalyze
{ 
    public sealed record AssignRoomDateOccupancyAnalyzeResponse
    {

        public List<AssignRoomDateOccupancyAnalyzeOwnerInfo> ownerInfo { get; set; }

        public List<AssignRoomDateOccupancyAnalyzeGuestInfo> GuestFutureInfo { get; set; }

    }

    public sealed record AssignRoomDateOccupancyAnalyzeOwnerInfo
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

        public string? RoomNumber { get; set; }

        public DateTime? InDate { get; set; }

 


    }



    public sealed record AssignRoomDateOccupancyAnalyzeGuestInfo
    {
        public int? Id { get; set; }
        public int? SAPID { get; set; }

        public string? Lastname { get; set; }
        public string? Firstname { get; set; }

        public int? Gender { get; set; }

        public string? EmployerName { get; set; }


        public string? PositionName { get; set; }

        public string? DepartmentName { get; set; }

        public int? DepartmentId { get; set; }

        public string? PeopleTypeName { get; set; }


        public bool? RoomOwner { get; set; }

        public string? ShiftCode { get; set; }
        public int? ShiftId { get; set; }


        public DateTime? OutDate { get; set; }


        public DateTime? GuestEventDate { get; set; }


        // public List<DateTime> DateInfo { get; set; }




    }


}