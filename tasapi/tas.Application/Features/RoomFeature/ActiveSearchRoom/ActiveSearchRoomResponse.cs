
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Text.Json;
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Domain.Common;

namespace tas.Application.Features.RoomFeature.ActiveSearchRoom
{

    public sealed record ActiveSearchRoomResponse : BasePaginationResponse<ActiveSearchRoom>
    {

    }




    public sealed record ActiveSearchRoom
    {
        public int? RoomId { get; set; }
        public string? RoomNumber { get; set; }

        public bool hasOwner { get; set; }

        public string? RoomType { get; set; }

        public int? Private { get; set; }

        public int? VirtualRoom {get; set;}

        public int? BedCount { get; set; }

        public int? MonthOccupancy { get; set; }

        public int? RoowOwners { get; set; }


        [NotMapped]
        public Dictionary<string, ActiveSearchRoomResponseDateInfo> OccupancyData { get; set; }

        [NotMapped]
        public Dictionary<string, ActiveSearchRoomResponseDateInfo> AnotherRoomData { get; set; }

    }

    public sealed record ActiveSearchRoomResponseDate
    {
        public string CurrentDate { get; set; }

        public ActiveSearchRoomResponseDateInfo DateInfo {get; set;}


    }

    public sealed record ActiveSearchRoomResponseDateInfo
    {
        public int? BedCount { get; set; }

        public int? ActiveBeds { get; set; }


    }

}