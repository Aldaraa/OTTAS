using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.PositionFeature.BulkDownloadPositionEmployees
{
    public sealed record BulkDownloadPositionEmployeesRequest(int PositionId) : IRequest<BulkDownloadPositionEmployeesResponse>;
}
