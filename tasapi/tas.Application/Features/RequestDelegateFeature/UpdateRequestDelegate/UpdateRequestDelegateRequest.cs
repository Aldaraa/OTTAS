using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDelegateFeature.UpdateRequestDelegate
{
    public sealed record UpdateRequestDelegateRequest(int Id, int fromEmployeeId, int toEmployeeId, DateTime startDate, DateTime endDate) : IRequest;
}
