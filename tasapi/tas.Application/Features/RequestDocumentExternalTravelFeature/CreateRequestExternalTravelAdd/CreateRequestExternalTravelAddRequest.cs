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

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestExternalTravelAdd
{

    public sealed record CreateRequestExternalTravelAddRequest(
        CreateRequestDocumentExternalTravelDocument documentData,
        CreateRequestDocumentExternalTravelData flightData,
        List<CreateRequestDocumentExternalTravelAttachment> Files


        ) : IRequest<int>;

    public sealed record CreateRequestDocumentExternalTravelDocument
    (
        string? Comment,
        int EmployeeId,
        string Action,
        int? AssignedEmployeeId,
        int NextGroupId

    );

    public sealed record CreateRequestDocumentExternalTravelData
    (
        int FirstScheduleId,
        int? LastScheduleId,
        int departmentId,
        int employerId,
        int positionId,
        int costcodeId,
        string? Reason

    );
    public sealed record CreateRequestDocumentExternalTravelAttachment
    (
        int  FileAddressId,
        string? Comment,
        int? IncludeEmail

    );



}
