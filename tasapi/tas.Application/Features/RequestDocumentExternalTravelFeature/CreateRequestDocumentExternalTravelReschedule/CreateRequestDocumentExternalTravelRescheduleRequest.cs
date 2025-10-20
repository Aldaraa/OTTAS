using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestDocumentExternalTravelReschedule
{

    public sealed record CreateRequestDocumentExternalTravelRescheduleRequest(
        CreateRequestDocumentExternalTravelRescheduleDocument documentData,
        CreateRequestDocumentExternalTravelRescheduleData flightData,
        List<CreateRequestDocumentExternalTravelRescheduleAttachment> Files


        ) : IRequest<int>;

    public sealed record CreateRequestDocumentExternalTravelRescheduleDocument
    (
        string? Comment,
        int EmployeeId,
        string Action,
        int? AssignedEmployeeId,
        int NextGroupId

    );

    public sealed record CreateRequestDocumentExternalTravelRescheduleData
    (
        int ScheduleId,
        int oldTransportId,
        int? DepartmentId,
        int? CostCodeId

    );
    public sealed record CreateRequestDocumentExternalTravelRescheduleAttachment
    (
        int  FileAddressId,
        string? Comment,
        int? IncludeEmail

    );



}
