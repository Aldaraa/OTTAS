using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysResponseTimeFeature.CreateSysResponseTime
{
    public sealed record CreateSysResponseTimeRequest(int? ResponseTime, string Path) : IRequest;
}
