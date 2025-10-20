using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.GetRequestDocumentProfileChange
{
    public sealed record GetRequestDocumentProfileChangeRequest(int DocumentId) : IRequest<GetRequestDocumentProfileChangeResponse>;
}
