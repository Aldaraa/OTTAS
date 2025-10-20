
using MediatR;
using System.Drawing.Printing;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelReschedule
{
    public sealed record GetRequestDocumentSiteTravelRescheduleResponse
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


        public GetRequestDocumentSiteTravelRescheduleTravel TravelData { get; set; }

    }


  
    public record GetRequestDocumentSiteTravelRescheduleTravel
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


        public int? RoomId { get; set; }

        public string? RoomnNumber { get; set; }




        public int? CampId { get; set; }


        public int? RoomTypeId { get; set; }


        public int? shiftId { get; set; }

        public string? Reason { get; set; }



        public int? ReScheduleFromLocationid { get; set; }

        public int? ReScheduleToLocationid { get; set; }


        public int? ExistingFromLocationid { get; set; }

        public int? ExistingToLocationid { get; set; }



        public string? ExistingScheduleIdDescr { get; set; }

        public string? ReScheduleIdDescr { get; set; }


        public string? ExistingTransportMode { get; set; }

        public string? ReScheduleTransportMode { get; set; }

        public int? ExistingScheduleIdNoShow { get; set; }


        public int? ReScheduleGoShow { get; set; }





    }




}