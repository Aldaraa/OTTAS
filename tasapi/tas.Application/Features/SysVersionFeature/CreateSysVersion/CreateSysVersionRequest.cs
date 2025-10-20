using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysVersionFeature.CreateSysVersion
{
    public sealed record CreateSysVersionRequest(string Version, string ReleaseNote) : IRequest;
}
