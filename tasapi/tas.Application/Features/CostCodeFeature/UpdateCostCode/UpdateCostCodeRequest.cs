using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.CostCodeFeature.UpdateCostCode
{
    public sealed record UpdateCostCodeRequest(int Id, string Code, string Description,  string Number, int Active) : IRequest;
}
