using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.DeleteEmployeeTransportBulk
{
    public sealed record DeleteEmployeeTransportBulkRequest(List<int> employeeIds, DateTime OnsiteDate) : IRequest;
}
