using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestAirportFeature.SearchRequestAirport
{
    public sealed record SearchRequestAirportRequest(string keyword) : IRequest<List<SearchRequestAirportResponse>>;
}
