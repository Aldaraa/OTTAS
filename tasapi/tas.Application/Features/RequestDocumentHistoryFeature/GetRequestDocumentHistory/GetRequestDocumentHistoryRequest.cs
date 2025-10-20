using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentHistoryFeature.GetRequestDocumentHistory
{
    public sealed record GetRequestDocumentHistoryRequest(int DocumentId) : IRequest<List<GetRequestDocumentHistoryResponse>>;
}
