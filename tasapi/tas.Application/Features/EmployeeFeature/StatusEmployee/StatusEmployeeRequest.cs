using MediatR;
using System.Reflection;
using System.Runtime.CompilerServices;
using tas.Domain.Common;

namespace tas.Application.Features.EmployeeFeature.StatusEmployee
{
    public sealed record StatusEmployeeRequest(int? employeeId, DateTime currentDate) : IRequest<StatusEmployeeResponse>;
}

