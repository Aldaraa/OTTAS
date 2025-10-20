
using MediatR;
using System.Drawing.Printing;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelReschedule
{
    public sealed record GetRequestDocumentExternalTravelRescheduleResponse
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


        public GetRequestDocumentExternalTravelRescheduleTravel TravelData { get; set; }

    }


  
    public record GetRequestDocumentExternalTravelRescheduleTravel
    {
        public int Id { get; set; }
        public int ExistingScheduleId { get; set; }
        public DateTime? ExistingScheduleDate { get; set; }
        public string? ExistingScheduleDirection { get; set; }

        public string? ExistingScheduleDescription { get; set; }
        public int ReScheduleId { get; set; }

        public DateTime? ReScheduleDate { get; set; }

        public string? ReScheduleDirection { get; set; }

        public string? ReScheduleDescription { get; set; }


        public int? ReScheduleAvailableSeats { get; set; }


    }




}