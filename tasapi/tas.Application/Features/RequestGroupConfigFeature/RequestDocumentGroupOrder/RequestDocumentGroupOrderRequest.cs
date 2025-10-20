using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupOrder
{
    public sealed record RequestDocumentGroupOrderRequest(string document, List<int> Ids) : IRequest;
}
