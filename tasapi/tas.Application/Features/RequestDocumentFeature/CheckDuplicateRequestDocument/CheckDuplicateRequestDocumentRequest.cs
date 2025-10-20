using MediatR;
using System.Drawing;

namespace tas.Application.Features.RequestDocumentFeature.CheckDuplicateRequestDocument
{
    public sealed record CheckDuplicateRequestDocumentRequest(string DocumentType, int EmployeeId, string? documentTag, DateTime? startDate, DateTime? endDate) : IRequest<List<CheckDuplicateRequestDocumentResponse>>;
}
