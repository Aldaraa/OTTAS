using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.GetEmployeeTransportAll
{
    public sealed record GetEmployeeTransportAllRequest(int employeeId, DateTime startDate) : IRequest<List<GetEmployeeTransportAllResponse>>;
}
