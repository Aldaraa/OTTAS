using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.DeleteRequestNonSiteTravelOption
{
    public sealed record DeleteRequestNonSiteTravelOptionRequest(int Id) : IRequest;
}
