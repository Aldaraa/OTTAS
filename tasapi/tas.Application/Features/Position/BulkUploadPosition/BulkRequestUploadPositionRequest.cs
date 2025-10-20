using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.PositioneFeature.BulkUploadPosition
{
    public sealed record BulkUploadPositionRequest(IFormFile BulkPositionFile) : IRequest<Unit>;
}
