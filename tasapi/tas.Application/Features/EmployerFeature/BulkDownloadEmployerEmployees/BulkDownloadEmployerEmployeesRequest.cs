using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployerFeature.BulkDownloadEmployerEmployees
{
    public sealed record BulkDownloadEmployerEmployeesRequest(int EmployerId) : IRequest<BulkDownloadEmployerEmployeesResponse>;
}
