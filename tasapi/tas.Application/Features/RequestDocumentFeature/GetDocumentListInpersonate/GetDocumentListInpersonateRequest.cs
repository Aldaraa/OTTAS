using MediatR;
using tas.Domain.Common;

namespace tas.Application.Features.RequestDocumentFeature.GetDocumentListInpersonate
{
    public sealed record GetDocumentListInpersonateRequest(int inpersonateUserId) :  IRequest<List<GetDocumentListInpersonateResponse>>;


}
