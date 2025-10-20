using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ColorFeature.CreateColor
{
    public sealed record CreateColorRequest(string Code, string Description, int Active) : IRequest;
}
