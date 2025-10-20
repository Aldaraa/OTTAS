using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployerFeature.BulkDownloadEmployer
{
    public sealed record BulkDownloadEmployerRequest(List<int> EmployerIds) : IRequest<BulkDownloadEmployerResponse>;
}
