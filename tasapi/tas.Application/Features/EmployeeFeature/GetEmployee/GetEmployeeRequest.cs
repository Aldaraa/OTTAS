using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.GetEmployee
{
    public sealed record GetEmployeeRequest(int Id) : IRequest<GetEmployeeResponse>;
}
