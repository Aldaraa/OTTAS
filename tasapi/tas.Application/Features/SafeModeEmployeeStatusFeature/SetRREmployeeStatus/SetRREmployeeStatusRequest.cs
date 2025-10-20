using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SafeModeEmployeeStatusFeature.SetRREmployeeStatus
{
    public sealed record SetRREmployeeStatusRequest(
        int EmployeeId,
        DateTime EventDate

        ) : IRequest<int>;
}
