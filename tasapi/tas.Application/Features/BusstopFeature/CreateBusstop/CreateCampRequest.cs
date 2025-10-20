using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.BusstopFeature.CreateBusstop
{
    public sealed record CreateBusstopRequest(string Description, int Active) : IRequest;
}
