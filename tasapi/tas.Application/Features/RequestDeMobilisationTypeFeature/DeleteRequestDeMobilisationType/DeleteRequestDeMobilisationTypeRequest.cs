using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDeMobilisationTypeFeature.DeleteRequestDeMobilisationType
{
    public sealed record DeleteRequestDeMobilisationTypeRequest(int Id) : IRequest;
}
