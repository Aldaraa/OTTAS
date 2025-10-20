using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.UpdateRequestNonSiteTravelOption
{
    public sealed record UpdateRequestNonSiteTravelOptionRequest(int Id, int Selected) : IRequest;
}
