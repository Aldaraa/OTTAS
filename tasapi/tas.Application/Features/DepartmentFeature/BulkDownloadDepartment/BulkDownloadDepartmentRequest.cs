using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentFeature.BulkDownloadDepartment
{
    public sealed record BulkDownloadDepartmentRequest(List<int> DepartmentIds) : IRequest<BulkDownloadDepartmentResponse>;
}
