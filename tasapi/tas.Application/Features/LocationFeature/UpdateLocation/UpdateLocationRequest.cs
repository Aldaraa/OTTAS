using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.LocationFeature.UpdateLocation
{
    public sealed record UpdateLocationRequest(int Id, string Code, string description, int onSite, int Active) : IRequest;
}
