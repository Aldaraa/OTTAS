using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tas.Application.Features.RequestDocumentFeature.ApproveRequestDocument
{

    public sealed record ApproveRequestDocumentRequest(
        int Id, int? assignEmployeeId, string? Comment, int NextGroupId, int? ImpersonateUserId

        ) : IRequest;



}
