using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.CostCodeFeature.DeleteCostCode
{
    public sealed record DeleteCostCodeRequest(int Id) : IRequest;
}
