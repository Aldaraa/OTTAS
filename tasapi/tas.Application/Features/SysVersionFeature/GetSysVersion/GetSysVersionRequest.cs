using MediatR;
using tas.Application.Features.SysVersionFeature.GetSysVersion;

namespace tas.Application.Features.SysVersionFeature.GetSysVersion
{
    public sealed record GetSysVersionRequest : IRequest<GetSysVersionResponse>;
}
