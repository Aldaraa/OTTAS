using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tas.Application.Features.RequestDocumentDeMobilisationFeature.UpdateRequestDocumentDeMobilisation
{

    public sealed record UpdateRequestDocumentDeMobilisationRequest(
            int? Id,
            DateTime CompletionDate,
            int EmployerId,
            int RequestDeMobilisationTypeId,
           string? Comment
        ) : IRequest;






}
