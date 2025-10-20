using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestAirportFeature.UpdateRequestAirport
{
    public sealed record UpdateRequestAirportRequest(int Id, string Code, string Description, string Country) : IRequest;
}
