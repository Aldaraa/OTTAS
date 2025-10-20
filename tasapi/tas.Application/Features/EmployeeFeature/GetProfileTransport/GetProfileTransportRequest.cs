using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.GetProfileTransport
{
    public sealed record GetProfileTransportRequest(int EmployeeId) : IRequest<List<GetProfileTransportResponse>>;
}
