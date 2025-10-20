using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupFeature.CreateRequestGroup
{
    public sealed record CreateRequestGroupRequest(string Description) : IRequest;
}
