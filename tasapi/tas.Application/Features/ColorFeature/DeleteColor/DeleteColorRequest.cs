using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ColorFeature.DeleteColor
{
    public sealed record DeleteColorRequest(int Id) : IRequest;
}
