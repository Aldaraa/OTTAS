using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupFeature.DeleteRequestGroup
{
    public sealed record DeleteRequestGroupRequest(int Id) : IRequest;
}
