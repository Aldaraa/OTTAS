using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestTravelPurposeFeature.DeleteRequestTravelPurpose
{
    public sealed record DeleteRequestTravelPurposeRequest(int Id) : IRequest;
}
