using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RoomAssignmentFeature.CreateRoomAssignment
{

    public class CreateRoomAssignmentResponse
    {
      //  public DateTime EventDate { get; set; }
        public List<CreateRoomAssignmentResponseGuest> Guests { get; set; }

        public List<CreateRoomAssignmentRoomByDatesBedData> BedInfo { get; set; }


    }


    public sealed class CreateRoomAssignmentRoomByDatesBedData
    {

        public DateTime? EventDate { get; set; }

        public string? RoomNumber { get; set; }

        public string? BedDescr { get; set; }

        public int? BedId { get; set; }
    }

    public class CreateRoomAssignmentResponseGuest
    {
        public DateTime? EventDate { get; set; }
        public int EmployeeId { get; set; }

        public string? Firstname { get; set; }
        public string? Lastname { get; set; }

        public int? Gender { get; set; }

        public int? SAPID { get; set; }

        public string? DepartmentName { get; set; }

        public string? PeopleTypeName { get; set; }

        public string? PositionName { get; set; }

        public string? EmployerName { get; set; }






    }
}
