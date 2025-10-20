using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.BusstopFeature.DeleteBusstop
{
    public sealed record DeleteBusstopRequest(int Id) : IRequest;
}
