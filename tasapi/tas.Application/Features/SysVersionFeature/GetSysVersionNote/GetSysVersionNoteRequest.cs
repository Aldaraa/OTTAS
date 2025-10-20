using MediatR;
using tas.Application.Features.SysVersionNoteFeature.GetSysVersionNote;

namespace tas.Application.Features.SysVersionNoteFeature.GetSysVersionNote
{
    public sealed record GetSysVersionNoteRequest(int Id) : IRequest<GetSysVersionNoteResponse>;
}
