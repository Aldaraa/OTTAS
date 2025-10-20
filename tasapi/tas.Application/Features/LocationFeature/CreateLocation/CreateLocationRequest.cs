using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.LocationFeature.CreateLocation
{
    public sealed record CreateLocationRequest(string? Code, string Description, int onSite) : IRequest;
}
