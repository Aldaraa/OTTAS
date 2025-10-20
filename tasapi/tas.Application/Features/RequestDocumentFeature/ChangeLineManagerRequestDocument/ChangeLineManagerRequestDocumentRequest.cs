using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tas.Application.Features.RequestDocumentFeature.ChangeLineManagerRequestDocument
{

    public sealed record ChangeLineManagerRequestDocumentRequest(
        int Id, int newAssignEmployeeId

        ) : IRequest;



}
