
using tas.Domain.Common;

namespace tas.Application.Features.RoomOwnerAndLockFeature.DateProfileRoomOwnerAndLock
{

    public sealed record DateProfileRoomOwnerAndLockResponse { 

        public List<DateProfileRoomOwnerAndLockResponseOwnerEmployee> OwnerFutureBooking { get; set; }

        public List<DateProfileRoomOwnerAndLockLockResponseDateEmployee> LockedEmployees { get; set; }

    }





    public sealed record DateProfileRoomOwnerAndLockLockResponseDateEmployee
    {
        public int Id { get; set; }
        public string? FullName { get; set; }

        public int? EmployeeId { get; set; }

        public bool? RoomOwner { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }


        public int? SAPID { get; set; }
        public string? Lastname { get; set; }

        public string? Firstname { get; set; }

        public string? PeopleTypeCode { get; set; }

        public int? Gender { get; set; }
        public int? HotelCheck { get; set; }

        public string? EmployerName { get; set; }

        public string? DepartmentName { get; set; }
        public int? DepartmentId { get; set; }


        public int DocumentId { get; set; }

        public string? DocumentTag { get; set; }



    }




    public sealed record DateProfileRoomOwnerAndLockResponseDateEmployeeBookingHistory
    {
        public int? RoomOwnerAndLockId { get; set; }

        public string? RoomOwnerAndLockNumber { get; set; }


        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

    }

    public sealed record DateProfileRoomOwnerAndLockResponseOwnerEmployee
    {
        public int Id { get; set; }
        public string? FullName { get; set; }

        public int? EmployeeId { get; set; }

        public int? RoomOwnerAndLockId { get; set; }

        public string? RoomNumber { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? SAPID { get; set; }
        public string? Lastname { get; set; }

        public string? Firstname { get; set; }

        public string? PeopleTypeCode { get; set; }

        public int? Gender { get; set; }
        public int? HotelCheck { get; set; }

        public string? EmployerName { get; set; }

        public string? DepartmentName { get; set; }
        public int? DepartmentId { get; set; }


        public string? ShiftCode { get; set; }


    }






}