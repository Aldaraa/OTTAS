using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.BulkDownloadEmployee
{
    public sealed record BulkDownloadEmployeeRequest(List<int> EmpIds) : IRequest<BulkDownloadEmployeeResponse>;
}
