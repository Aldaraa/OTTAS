using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDeMobilisationTypeFeature.CreateRequestDeMobilisationType
{
    public sealed record CreateRequestDeMobilisationTypeRequest(string Code, string Description) : IRequest;
}
