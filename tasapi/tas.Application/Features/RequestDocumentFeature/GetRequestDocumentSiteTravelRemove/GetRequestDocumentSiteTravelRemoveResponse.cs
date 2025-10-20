
using MediatR;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelRemove
{
    public sealed record GetRequestDocumentSiteTravelRemoveResponse
    {
        public int? DaysAway { get; set; }
        public int Id { get; set; }

        public string? CurrentStatus { get; set; }
        public string? DocumentType { get; set; }

        public string? RequesterFullName { get; set; }

        public string? RequesterMobile { get; set; }

        public string? RequesterMail { get; set; }

        public string? AssignedEmployeeFullName { get; set; }

        public DateTime? RequestedDate { get; set; }

        public int? RequestUserId { get; set; }

        public string? EmployeeFullName { get; set; }

        public int? EmployeeActive { get; set; }

        public int? EmployeeId { get; set; }

        public string? UpdatedInfo { get; set; }

        public int? AssignedEmployeeId { get; set; }

        public int? AssignedRouteConfigId { get; set; }
        public int? DelegateEmployeeId { get; set; }


        public GetRequestDocumentSiteTravelRemoveTravel TravelData { get; set; }

    }


  
    public record GetRequestDocumentSiteTravelRemoveTravel
    {
        public int Id { get; set; }
        public int FirstScheduleId { get; set; }
        public DateTime? FirstScheduleDate { get; set; }
        public string? FirstScheduleDirection { get; set; }

        public string? LastScheduleDescription { get; set; }

        public string? FirstScheduleDescription { get; set; }


        public int LastScheduleId { get; set; }

        public DateTime? LastScheduleDate { get; set; }

        public string? LastScheduleDirection { get; set; }

        public int? RoomId { get; set; }

        public string? RoomNumber { get; set; }


        public int? CostCodeId { get; set; }


        public int? CampId { get; set; }


        public int? RoomTypeId { get; set; }


        public int? shiftId { get; set; }

       public string? Reason { get; set; }

        public string? LastScheduleIdDescr { get; set; }

        public string? FirstScheduleIdDescr { get; set; }

        public int? FirstScheduleNoShow { get; set; }

        public int? LastScheduleNoShow { get; set; }





    }




}