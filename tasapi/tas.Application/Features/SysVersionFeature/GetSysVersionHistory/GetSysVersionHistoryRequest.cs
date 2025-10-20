using MediatR;
using tas.Application.Features.SysVersionHistoryFeature.GetSysVersionHistory;

namespace tas.Application.Features.SysVersionHistoryFeature.GetSysVersionHistory
{
    public sealed record GetSysVersionHistoryRequest : IRequest<List<GetSysVersionHistoryResponse>>;
}
