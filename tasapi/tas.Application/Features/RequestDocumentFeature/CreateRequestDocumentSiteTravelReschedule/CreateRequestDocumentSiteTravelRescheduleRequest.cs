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

namespace tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelReschedule
{

    public sealed record CreateRequestDocumentSiteTravelRescheduleRequest(
        CreateRequestDocumentSiteTravelRescheduleDocument documentData,
        CreateRequestDocumentSiteTravelRescheduleData flightData,
        List<CreateRequestDocumentSiteTravelRescheduleAttachment> Files,
        bool skipMailNotification



        ) : IRequest<int>;

    public sealed record CreateRequestDocumentSiteTravelRescheduleDocument
    (
        string? Comment,
        int EmployeeId,
        string Action,
        int? AssignedEmployeeId,
        int NextGroupId

    );

    public sealed record CreateRequestDocumentSiteTravelRescheduleData
    (
        int existingScheduleId,
        int reScheduleId,
        int shiftId,
        string? Reason,
        int? ExistingScheduleIdNoShow,
        int? ReScheduleGoShow


    );
    public sealed record CreateRequestDocumentSiteTravelRescheduleAttachment
    (
        int  FileAddressId,
        string? Comment,
        int? IncludeEmail

    );



}
