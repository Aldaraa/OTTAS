using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.CostCodeFeature.BulkDownloadCostCodeEmployees
{
    public sealed record BulkDownloadCostCodeEmployeesRequest(int CostCodeId) : IRequest<BulkDownloadCostCodeEmployeesResponse>;
}
