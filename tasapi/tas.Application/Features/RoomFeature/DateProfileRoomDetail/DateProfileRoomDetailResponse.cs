
namespace tas.Application.Features.RoomFeature.DateProfileRoomDetail
{
    public sealed record DateProfileRoomDetailResponse
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

        public int? DepartmentId { get; set; }





    }







}