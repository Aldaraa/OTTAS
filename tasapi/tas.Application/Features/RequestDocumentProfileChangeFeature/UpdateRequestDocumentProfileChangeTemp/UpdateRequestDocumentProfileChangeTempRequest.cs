using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.UpdateRequestDocumentProfileChangeTemp
{

    public sealed record UpdateRequestDocumentProfileChangeTempRequest(
        int Id,
        int? EmployerId,
        int? CostCodeId,
        int? DepartmentId,
        int? PositionId,
    DateTime? StartDate,
    DateTime? EndDate,
    int? Permanent
        ) : IRequest;






}
