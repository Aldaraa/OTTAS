using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.CreateRequestDocumentProfileChangeTemp
{

    public sealed record CreateRequestDocumentProfileChangeTempRequest(
        CreateRequestDocumentProfileChangeTempEmployee Employee,
        CreateRequestDocumentProfileChangeTempData changeRequestData 
        ) : IRequest<int>;

    public sealed record CreateRequestDocumentProfileChangeTempData
    (
        string? comment,
        int? assignedEmployeeId,
        string action,
        int nextGroupId


    );
    public sealed record CreateRequestDocumentProfileChangeTempEmployee
    (
         int EmployeeId,
         int? EmployerId,
         int? CostCodeId,
         int? DepartmentId,
         int? PositionId,
         DateTime? StartDate,
         DateTime? EndDate,
         int? Permanent
    );





}
