using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupFeature.UpdateRequestGroup
{
    public sealed record UpdateRequestGroupRequest(int Id, string Description) : IRequest;
}
