using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroup
{
    public sealed record RequestDocumentGroupRequest(string document) : IRequest<List<RequestDocumentGroupResponse>>;
}
