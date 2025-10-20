using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDeMobilisationTypeFeature.UpdateRequestDeMobilisationType
{
    public sealed record UpdateRequestDeMobilisationTypeRequest(int Id, string Code, string Description) : IRequest;
}
