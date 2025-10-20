
namespace tas.Application.Features.RoomFeature.FindRoomDateOccupancyAnalyze
{ 
    public sealed record FindRoomDateOccupancyAnalyzeResponse
    {

        public List<FindRoomDateOccupancyAnalyzeCurrentInfo> currentInfo { get; set; }

        public List<FindRoomDateOccupancyAnalyzeHistoryInfo> HistoryInfo { get; set; }

    }

    public sealed record FindRoomDateOccupancyAnalyzeCurrentInfo
    {
        public int? Id { get; set; }
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

        public bool? RoomOwner { get; set; }

        public DateTime? OutDate { get; set; }


        // public List<FindRoomDateOccupancyAnalyzeCurrentInfoDatesStatus> DateInfo { get; set; }



    }

    //public record FindRoomDateOccupancyAnalyzeCurrentInfoDatesStatus
    //{
    //    public DateTime? EventDate { get; set; }

    //    public int? ShiftId { get; set; }

    //    public string? ShiftCode { get; set; }

    //    public string? ShiftColor { get; set; }
        

    //}



    public sealed record FindRoomDateOccupancyAnalyzeHistoryInfo
    {
        public int? Id { get; set; }
        public int? SAPID { get; set; }

        public string? Lastname { get; set; }
        public string? Firstname { get; set; }

        public int? Gender { get; set; }

        public string? EmployerName { get; set; }


        public string? PositionName { get; set; }

        public string? DepartmentName { get; set; }

        public string? PeopleTypeName { get; set; }

        public int? HotelCheck { get; set; }

        public string? RoomNumber { get; set; }

        public bool? RoomOwner { get; set; }

     //   public List<DateTime?> RoomDateLog { get; set; }



    }


}