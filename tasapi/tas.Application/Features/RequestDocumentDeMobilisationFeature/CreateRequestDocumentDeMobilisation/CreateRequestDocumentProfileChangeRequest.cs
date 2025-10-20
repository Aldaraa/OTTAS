using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tas.Application.Features.RequestDocumentDeMobilisationFeature.CreateRequestDocumentDeMobilisation
{

    public sealed record CreateRequestDocumentDeMobilisationRequest(
        CreateRequestDocumentDeMobilisation DeMobilisationData,
        CreateRequestDocumentDeMobilisationData RequestData 
        ) : IRequest<int>;

    public sealed record CreateRequestDocumentDeMobilisationData
    (
        string? Comment,
        int? AssignedEmployeeId,
        string Action,
        int EmployeeId,
        int nextGroupId


    );
    
    public sealed record CreateRequestDocumentDeMobilisation
        (
            DateTime? CompletionDate,
            int? EmployerId,
            int? RequestDeMobilisationTypeId,
           string? Comment
        );
    




}
