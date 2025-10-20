using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.LocationFeature.DeleteLocation
{
    public sealed record DeleteLocationRequest(int Id) : IRequest;
}
