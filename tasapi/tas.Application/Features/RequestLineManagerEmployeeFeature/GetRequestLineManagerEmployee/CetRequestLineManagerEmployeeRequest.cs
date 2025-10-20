using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentRoute;

namespace tas.Application.Features.RequestLineManagerEmployeeFeature.GetRequestLineManagerEmployee
{
    public sealed record GetRequestLineManagerEmployeeRequest : IRequest<List<GetRequestLineManagerEmployeeResponse>>;
}
