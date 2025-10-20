using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentDeMobilisationFeature.GetRequestDocumentDeMobilisation
{
    public sealed record GetRequestDocumentDeMobilisationResponse
    {

        public int? DaysAway { get; set; }
        public int Id { get; set; }

        public string? CurrentStatus { get; set; }
        public string? DocumentType { get; set; }

        public string? RequesterFullName { get; set; }

        public string? AssignedEmployeeFullName { get; set; }

        public string? RequesterMobile { get; set; }

        public string? RequesterMail { get; set; }

        public DateTime? RequestedDate { get; set; }

        public int? RequestUserId { get; set; }

        public string? EmployeeFullName { get; set; }

        public int? EmployeeId { get; set; }

        public string? UpdatedInfo { get; set; }

        public int? AssignedEmployeeId { get; set; }

        public int? AssignedRouteConfigId { get; set; }

        public int? DelegateEmployeeId { get; set; }


        public GetRequestDocumentDeMobilisationInfo info { get; set; }
    }

    public sealed record GetRequestDocumentDeMobilisationInfo
    {
        public int Id { get; set; }

        public int? DocumentId { get; set; }

        public DateTime? CompletionDate { get; set; }

        public int? EmployerId { get; set; }

        public string? Comment { get; set; }
        public int? RequestDeMobilisationTypeId { get; set; }
    }









}
