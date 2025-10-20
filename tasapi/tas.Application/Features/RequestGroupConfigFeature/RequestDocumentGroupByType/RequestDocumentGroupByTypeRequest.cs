using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupByType
{
    public sealed record RequestDocumentGroupByTypeRequest(string documentType, int? empId) : IRequest<RequestDocumentGroupByTypeResponse>;
}
