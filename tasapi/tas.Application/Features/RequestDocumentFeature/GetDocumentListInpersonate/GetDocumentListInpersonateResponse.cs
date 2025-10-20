
using MediatR;
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentFeature.GetDocumentListInpersonate
{


    public sealed record GetDocumentListInpersonateResponse 
    {
        public int? DaysAway { get; set; }
        public int Id { get; set; }

        public string? CurrentStatus { get; set; }
        public string? DocumentType { get; set; }

        public string? Description { get; set; }

        public string? RequesterFullName { get; set; }

        public string? AssignedEmployeeFullName { get; set; }

        public string? EmployerName { get; set; }

        public DateTime? RequestedDate { get; set; }

        public string? EmployeeFullName { get; set; }

        public string? UpdatedInfo { get; set; }

        public int? EmployerId { get; set; }

        public string? DocumentTag { get; set; }


        public string? AssignedGroupName { get; set; }

        public string? CurrentStatusGroup { get; set; }
    }





}