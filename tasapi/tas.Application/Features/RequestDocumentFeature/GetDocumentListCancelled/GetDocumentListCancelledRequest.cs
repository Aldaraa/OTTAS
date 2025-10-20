using MediatR;
using tas.Domain.Common;

namespace tas.Application.Features.RequestDocumentFeature.GetDocumentListCancelled
{
    public sealed record GetDocumentListCancelledRequest(GetDocumentListCancelledModel model) : BasePagenationRequest, IRequest<GetDocumentListCancelledResponse>;

    public record GetDocumentListCancelledModel(
        int? Id,
        DateTime? startDate,
        DateTime? endDate,
        string? DocumentType,
        int? EmployerId,
        int? AssignedEmployeeId,
        int? RequestedEmployeeId,
        DateTime? LastModifiedDate,
        int? ApprovelType,
        string? Keyword
    );

}
