using MediatR;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.EmployeeTransportGoShow
{
    public sealed record EmployeeTransportGoShowRequest(int employeeId, int year) : IRequest<List<EmployeeTransportGoShowResponse>>;
}
