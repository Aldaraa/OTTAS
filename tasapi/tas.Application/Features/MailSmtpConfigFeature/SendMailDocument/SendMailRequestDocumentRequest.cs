using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.MailSmtpConfigFeature.SendMailRequestDocument
{
    public sealed record SendMailRequestDocumentRequest(
    int documentId
    ) : IRequest;
}
