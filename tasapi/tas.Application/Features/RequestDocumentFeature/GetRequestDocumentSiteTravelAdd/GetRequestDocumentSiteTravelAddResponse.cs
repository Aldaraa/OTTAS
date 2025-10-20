
using MediatR;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd
{
    public sealed record GetRequestDocumentSiteTravelAddResponse
    {
        public int? DaysAway { get; set; }
        public int Id { get; set; }

        public string? CurrentStatus { get; set; }
        public string? DocumentType { get; set; }

        public string? RequesterFullName { get; set; }

        public string? RequesterMobile { get; set; }

        public string? RequesterMail { get; set; }

        public int? RequestUserId { get; set; }

        public string? AssignedEmployeeFullName { get; set; }

        public DateTime? RequestedDate { get; set; }

        public string? EmployeeFullName { get; set; }

        public int? EmployeeActive { get; set; }

        public int? EmployeeId { get; set; }

        public string? UpdatedInfo { get; set; }

        public int? AssignedEmployeeId { get; set; }

        public int? AssignedRouteConfigId { get; set; }

        public int? DelegateEmployeeId { get; set; }

        public GetRequestDocumentSiteTravelAddTravel TravelData { get; set; }

    }


  
    public record GetRequestDocumentSiteTravelAddTravel
    {
        public int Id { get; set; }
        public int inScheduleId { get; set; }
        public DateTime? inScheduleDate { get; set; }
        public string? inScheduleDirection { get; set; }

        public int outScheduleId { get; set; }

        public string? INScheduleDescription { get; set; }

        public string? OUTScheduleDescription { get; set; }

        public string? INScheduleTransportMode { get; set; }

        public string? OUTScheduleTransportMode { get; set; }

        public int? inScheduleGoShow { get; set; }

        public int? outScheduleGoShow { get; set; }



        public DateTime? outScheduleDate { get; set; }

        public string? outScheduleDirection { get; set; }

        public int? RoomId { get; set; }

        public string? RoomNumber { get; set; }


        public int? CampId { get; set; }


        public int? RoomTypeId { get; set; }


        public int? shiftId { get; set; }

        public int? EmployerId { get; set; }
        public int? PositionId { get; set; }

        public int? DepartmentId { get; set; }

        public int? CostCodeId { get; set; }

        public string? Reason { get; set; }



        public string? inScheduleIdDescr { get; set; }
        public string? outScheduleIdDescr { get; set; }



    }




}