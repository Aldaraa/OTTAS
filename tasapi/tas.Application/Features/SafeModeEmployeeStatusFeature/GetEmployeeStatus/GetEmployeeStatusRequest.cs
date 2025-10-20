using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SafeModeEmployeeStatusFeature.GetEmployeeStatus
{
    public sealed record GetEmployeeStatusRequest(
        int EmployeeId,
        DateTime EventDate
        ) : IRequest<GetEmployeeStatusResponse>;
}
