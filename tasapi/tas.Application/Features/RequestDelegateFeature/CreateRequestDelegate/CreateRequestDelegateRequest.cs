using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDelegateFeature.CreateRequestDelegate
{
    public sealed record CreateRequestDelegateRequest(int fromEmployeeId, int toEmployeeId, DateTime startDate, DateTime endDate) : IRequest;
}
