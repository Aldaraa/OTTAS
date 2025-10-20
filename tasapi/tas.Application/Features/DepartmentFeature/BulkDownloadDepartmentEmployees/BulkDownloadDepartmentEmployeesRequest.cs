using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentFeature.BulkDownloadDepartmentEmployees
{
    public sealed record BulkDownloadDepartmentEmployeesRequest(int DepartmentId) : IRequest<BulkDownloadDepartmentEmployeesResponse>;
}
