using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupRemove
{
    public sealed record RequestDocumentGroupRemoveRequest(int Id) : IRequest;
}
