using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.CostCodeFeature.CreateCostCode
{
    public sealed record CreateCostCodeRequest(string Code, string Description,  string Number, int Active) : IRequest;
}
