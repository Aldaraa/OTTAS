using MediatR;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.EmployeeTransportNoShow
{
    public sealed record EmployeeTransportNoShowRequest(int employeeId, int year) : IRequest<List<EmployeeTransportNoShowResponse>>;
}
