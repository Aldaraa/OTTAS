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

namespace tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelRemove
{

    public sealed record CreateRequestDocumentSiteTravelRemoveRequest(
        CreateRequestDocumentSiteTravelRemoveDocument documentData,
        CreateRequestDocumentSiteTravelRemoveData flightData,
        List<CreateRequestDocumentSiteTravelRemoveAttachment> Files,
        bool skipMailNotification


        ) : IRequest<int>;

    public sealed record CreateRequestDocumentSiteTravelRemoveDocument
    (
        string? Comment,
        int EmployeeId,
        string Action,
        int? AssignedEmployeeId,
        int NextGroupId

    );

    public sealed record CreateRequestDocumentSiteTravelRemoveData
    (
        int FirstScheduleId,
        int LastScheduleId,
        int shiftId,
        int? CostCodeId,
        string? Reason,
        int? FirstScheduleNoShow,
        int? LastScheduleNoShow

    );
    public sealed record CreateRequestDocumentSiteTravelRemoveAttachment
    (
        int  FileAddressId,
        string? Comment,
        int? IncludeEmail

    );



}
