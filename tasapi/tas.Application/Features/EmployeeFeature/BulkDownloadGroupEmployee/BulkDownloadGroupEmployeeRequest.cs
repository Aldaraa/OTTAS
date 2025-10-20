using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.BulkDownloadGroupEmployee
{
    public sealed record BulkDownloadGroupEmployeeRequest(List<int> EmpIds) : IRequest<BulkDownloadGroupEmployeeResponse>;
}
