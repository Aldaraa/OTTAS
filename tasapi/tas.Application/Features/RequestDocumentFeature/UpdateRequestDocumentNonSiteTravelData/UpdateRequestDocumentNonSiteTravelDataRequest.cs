using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentNonSiteTravelData
{

    public sealed record UpdateRequestDocumentNonSiteTravelDataRequest(
        int RequestTravelAgentId,
        string RequestTravelAgentSureName,
        decimal Cost,
        decimal? HighestCost,
        decimal? Cost2,
        int DocumentId,
        int? SelectedOptionId,
        int? RequestTravelPurposeId
        ) : IRequest;
}
