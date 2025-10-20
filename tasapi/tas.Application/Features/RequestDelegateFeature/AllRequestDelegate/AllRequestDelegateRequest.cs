using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.RequestDelegateFeature.AllRequestDelegate
{
    public sealed record AllRequestDelegateRequest(int? fromEmpoyeeId, DateTime? startDate, DateTime? endDate) :  IRequest<List<AllRequestDelegateResponse>>;

}
