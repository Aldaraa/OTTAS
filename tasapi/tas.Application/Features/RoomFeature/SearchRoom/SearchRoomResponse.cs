
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.SearchRoom
{

    public sealed record SearchRoomResponse : BasePaginationResponse<RoomSearchResult>
    {

    }


    public sealed record RoomSearchResult
    {
        public int Id { get; set; }
        public string Number { get; set; }

        public int? BedCount { get; set; }
        
        public int? Private { get; set; }

        public int? CampId { get; set; }

        public string? CampName { get; set; }

        public int? RoomTypeId { get; set; }

        public string? RoomTypeName { get; set; }


        public int? VirtualRoom { get; set; }



    }
}