using MediatR;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.EmployeeDateTransport
{
    public sealed record EmployeeDateTransportRequest(int employeeId, DateTime currentDate, string direction) : IRequest<EmployeeDateTransportResponse>;
}
