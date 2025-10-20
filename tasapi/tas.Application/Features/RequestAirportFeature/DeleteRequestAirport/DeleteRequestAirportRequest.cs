using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestAirportFeature.DeleteRequestAirport
{
    public sealed record DeleteRequestAirportRequest(int Id) : IRequest;
}
