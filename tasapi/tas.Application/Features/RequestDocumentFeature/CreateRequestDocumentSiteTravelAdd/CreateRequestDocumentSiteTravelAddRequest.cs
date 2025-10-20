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

namespace tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelAdd
{

    public sealed record CreateRequestDocumentSiteTravelAddRequest(
        CreateRequestDocumentSiteTravelDocument documentData,
        CreateRequestDocumentSiteTravelData flightData,
        List<CreateRequestDocumentSiteTravelAttachment> Files,
        bool skipMailNotification




        ) : IRequest<int>;

    public sealed record CreateRequestDocumentSiteTravelDocument
    (
        string? Comment,
        int EmployeeId,
        string Action,
        int? AssignedEmployeeId,
        int NextGroupId

    );

    public sealed record CreateRequestDocumentSiteTravelData
    (
        int inScheduleId,
        int outScheduleId,
        int departmentId,
        int shiftId,
        int employerId,
        int positionId,
        int costcodeId,
        int? roomId,
        string? Reason,
        int? inScheduleGoShow,
        int? outScheduleGoShow

    );
    public sealed record CreateRequestDocumentSiteTravelAttachment
    (
        int  FileAddressId,
        string? Comment,
        int? IncludeEmail

    );



}
