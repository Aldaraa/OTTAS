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

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestDocumentExternalTravelRemove
{

    public sealed record CreateRequestDocumentExternalTravelRemoveRequest(
        CreateRequestDocumentExternalTravelRemoveDocument documentData,
        CreateRequestDocumentExternalTravelRemoveData flightData,
        List<CreateRequestDocumentExternalTravelRemoveAttachment> Files


        ) : IRequest<int>;

    public sealed record CreateRequestDocumentExternalTravelRemoveDocument
    (
        string? Comment,
        int EmployeeId,
        string Action,
        int? AssignedEmployeeId,
        int NextGroupId

    );

    public sealed record CreateRequestDocumentExternalTravelRemoveData
    (
        int TransportId

    );
    public sealed record CreateRequestDocumentExternalTravelRemoveAttachment
    (
        int  FileAddressId,
        string? Comment,
        int? IncludeEmail

    );



}
