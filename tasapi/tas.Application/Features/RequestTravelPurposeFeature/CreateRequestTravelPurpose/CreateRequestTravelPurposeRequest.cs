using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestTravelPurposeFeature.CreateRequestTravelPurpose
{
    public sealed record CreateRequestTravelPurposeRequest(string Code, string Description) : IRequest;
}
