using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.GetRequestDocumentProfileChangeTemp
{
    public sealed record GetRequestDocumentProfileChangeTempRequest(int DocumentId) : IRequest<GetRequestDocumentProfileChangeTempResponse>;
}
