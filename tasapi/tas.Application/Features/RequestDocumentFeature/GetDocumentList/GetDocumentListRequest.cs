using MediatR;
using tas.Domain.Common;

namespace tas.Application.Features.RequestDocumentFeature.GetDocumentList
{
    public sealed record GetDocumentListRequest(GetDocumentRequestModel model /*, List<GetDocumentRequestGroup> group*/) : BasePagenationRequest, IRequest<GetDocumentListResponse>;

    public record GetDocumentRequestModel(
        int? Id,
        DateTime? startDate,
        DateTime? endDate,
        string? DocumentType,
        int? EmployerId,
        int? AssignedEmployeeId,
        int? RequestedEmployeeId,
        string? RequestDocumentSearchCurrentStep,
        DateTime? LastModifiedDate,
        int? ApprovelType,
        string? Keyword

    );

    public record GetDocumentRequestGroup(
        string selector,
        bool? desc,
        bool? isExpanded
    );

}
