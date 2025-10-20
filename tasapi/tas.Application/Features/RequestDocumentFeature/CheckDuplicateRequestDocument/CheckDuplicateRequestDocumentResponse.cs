
using MediatR;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentFeature.CheckDuplicateRequestDocument
{
    public sealed record CheckDuplicateRequestDocumentResponse
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        
        public string? CurrentAction { get; set; }

        public string AssignedEmployee { get; set; }
 
        public DateTime? CreatedAt { get; set; }

        public string? DocumentType { get; set; }
        public string? DocumentTag { get; set; }


    }









}