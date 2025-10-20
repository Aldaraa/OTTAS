using MediatR;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.EmployeeExistingTransport
{
    public sealed record EmployeeExistingTransportRequest(int employeeId, DateTime startDate, DateTime endDate) : IRequest<List<EmployeeExistingTransportResponse>>;
}
