using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestTravelAgentFeature.CreateRequestTravelAgent
{
    public sealed record CreateRequestTravelAgentRequest(string Description) : IRequest;
}
