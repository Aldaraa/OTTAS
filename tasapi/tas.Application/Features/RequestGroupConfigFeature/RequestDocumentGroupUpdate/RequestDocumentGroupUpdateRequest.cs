using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupUpdate
{
    public sealed record RequestDocumentGroupUpdateRequest(string document, int groupId, string? RuleAction) : IRequest;
}
