using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.DeleteEmployeeTransport
{
    public sealed record DeleteEmployeeTransportRequest(int employeeId, DateTime OnsiteDate) : IRequest;
}
