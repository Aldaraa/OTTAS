using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestTravelAgentFeature.UpdateRequestTravelAgent
{
    public sealed record UpdateRequestTravelAgentRequest(int Id,  string Description) : IRequest;
}
