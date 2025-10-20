
using MediatR;
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentFeature.ExistingBookingRequestDocument
{

    public sealed record ExistingBookingRequestDocumentResponse 
    {

        public int EmployeeId { get; set; }

        public string? FullName  { get; set; }

        public int? SAPID { get; set; }

        public string? Employer { get; set; }

        public string? Department { get; set; }



        public List<ExistingBookingRequestDocumentOtherRequest> OtherRequest { get; set; }
        public List<ExistingBookingRequestDocumentPending> PendingRequest { get; set; }

        public List<ExistingBookingRequestDocumentSiteTravel> SiteTravel { get; set; }

        public List<ExistingBookingRequestDocumentNonSiteTravel> NonSiteTravel { get; set; }



    }


    public record ExistingBookingRequestDocumentOtherRequest
    {
        public int Id { get; set; }

        public string? CurrentStatus { get; set; }
        public string? DocumentType { get; set; }

        public string? DocumentTag { get; set; }


        public string? Description { get; set; }

        public string? RequesterFullName { get; set; }

        public string? AssignedEmployeeFullName { get; set; }

        public DateTime? RequestedDate { get; set; }

    }


    public record ExistingBookingRequestDocumentPending
    {
        public int Id { get; set; }

        public string? CurrentStatus { get; set; }
        public string? DocumentType { get; set; }

        public string? DocumentTag { get; set; }


        public string? Description { get; set; }

        public string? RequesterFullName { get; set; }

        public string? AssignedEmployeeFullName { get; set; }

        public DateTime? RequestedDate { get; set; }

    }


    public record ExistingBookingRequestDocumentSiteTravel
    {
        public int Id { get; set; }
        public DateTime? EventDate { get; set; }

        public string? Status { get; set; }

        public string? Code { get; set; }

        public string? Description { get; set; }

        public string? Direction { get; set; }
    }


    public record ExistingBookingRequestDocumentNonSiteTravel
    {
        public int Id { get; set; }
        public DateTime? TravelDate { get; set; }
        public string? FavorTime { get; set; }
        public int? ETD { get; set; }
        public int? DepartLocationId { get; set; }
        public string? DepartLocationName { get; set; }
        public int? ArriveLocationId { get; set; }
        public string? ArriveLocationName { get; set; }
        public string? Comment { get; set; }
        public int DocumentId { get; set; }
    }









}