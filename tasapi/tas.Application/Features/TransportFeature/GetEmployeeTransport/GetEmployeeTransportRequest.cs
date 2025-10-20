using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.GetEmployeeTransport
{
    public sealed record GetEmployeeTransportRequest(int employeeId, DateTime? startDate, DateTime? endDate) : IRequest<List<GetEmployeeTransportResponse>>;
}
