using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.BusstopFeature.UpdateBusstop
{
    public sealed record UpdateBusstopRequest(int Id,  string Description, int Active) : IRequest;
}
