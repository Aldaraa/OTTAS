
using MediatR;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelRemove
{
    public sealed record GetRequestDocumentExternalTravelRemoveResponse
    {
        public int? DaysAway { get; set; }
        public int Id { get; set; }

        public string? CurrentStatus { get; set; }
        public string? DocumentType { get; set; }

        public string? RequesterFullName { get; set; }

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


        public GetRequestDocumentExternalTravelRemoveTravel TravelData { get; set; }

    }


  
    public record GetRequestDocumentExternalTravelRemoveTravel
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public string? ScheduleDirection { get; set; }

        public string? ScheduleDescription { get; set; }



    }




}