using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeStatusFeature.ChangeRoomByDates
{
    public sealed class ChangeRoomByDatesResponse
    {
        public int Id { get; set; }
        public string? FullName { get; set; }

        public int? EmployeeId { get; set; }

        public bool? RoomOwner { get; set; }

        public int? SAPID { get; set; }
        public string? Lastname { get; set; }

        public string? Firstname { get; set; }

        public string? PeopleTypeCode { get; set; }

        public int? Gender { get; set; }
        public int? HotelCheck { get; set; }

        public string? EmployerName { get; set; }

        public string? DepartmentName { get; set; }



        public List<ChangeRoomByDatesBedData> BedInfo { get; set; }



    }

    public sealed class ChangeRoomByDatesBedData
    {

        public DateTime? EventDate { get; set; }

        public string? RoomNumber { get; set; }

        public string? BedDescr { get; set; }

        public int? BedId { get; set; }
    }


}