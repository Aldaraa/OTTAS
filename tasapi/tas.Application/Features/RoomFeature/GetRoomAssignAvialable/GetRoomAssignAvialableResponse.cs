
using tas.Domain.Common;

namespace tas.Application.Features.RoomFeature.GetRoomAssignAvialable
{ 
    public sealed record GetRoomAssignAvialableResponse  
    {

        public string? roomNumber { get; set; }
        public int? RoomId { get; set; }

        public int? BedCount { get; set; }

        public int? VirtulRoom { get; set; }

        public int? Employees { get; set; }

        public string? Descr { get; set; }

        public int? OwnerCount { get; set; }

       

    }








}