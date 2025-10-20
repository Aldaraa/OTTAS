using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCodeEmployees
{
    public sealed record BulkUploadPreviewCostCodeEmployeesRequest(IFormFile BulkCostCodeFile) : IRequest<BulkUploadPreviewCostCodeEmployeesResponse>;
}
