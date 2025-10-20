
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomAssignmentFeature.FindAvailableByDatesAssignment
{ 
    public sealed record FindAvailableByDatesAssignmentResponse  
    {

        public string? roomNumber { get; set; }
        public int? RoomId { get; set; }

        public int? BedCount { get; set; }

        public int? VirtulRoom { get; set; }
        public string? Descr { get; set; }

        public int? Employees { get; set; }

        public int? RoomOwners { get; set; }

        public DateTime? OwnerInDate{ get; set; }
    }



}