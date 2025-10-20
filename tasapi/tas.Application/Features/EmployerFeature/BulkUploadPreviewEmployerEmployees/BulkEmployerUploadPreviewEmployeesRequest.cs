using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployerFeature.BulkUploadPreviewEmployerEmployees
{
    public sealed record BulkEmployerUploadPreviewEmployeesRequest(IFormFile BulkEmployerFile) : IRequest<BulkEmployerUploadPreviewEmployeesResponse>;
}
