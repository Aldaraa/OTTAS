using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.GetEmployeeAccountHistory
{
    public sealed record GetEmployeeAccountHistoryRequest(int Id) : IRequest<List<GetEmployeeAccountHistoryResponse>>;
}
