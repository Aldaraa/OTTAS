
using tas.Domain.Common;

namespace tas.Application.Features.RoomFeature.DateProfileRoomExport
{


    public sealed record DateProfileRoomExportResponse 
    {
        public byte[] ExcelFile { get; set; }
    }



    public sealed record DateProfileRoomExportResponseDateEmployee
    {
        public string? FullName { get; set; }

        public int? EmployeeId { get; set; }

        public string? RoomOwner { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }


        public int? SAPID { get; set; }

        public string? PeopleTypeCode { get; set; }

        public string? Gender { get; set; }

        public string? EmployerName { get; set; }

        public string? DepartmentName { get; set; }

        public int? PeopleTypeId { get; set; }

        public string? Mobile { get; set; }



    }


    





}