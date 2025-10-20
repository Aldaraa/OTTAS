using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.DeleteEmployee
{
    public sealed record DeleteEmployeeRequest(int Id) : IRequest;
}
