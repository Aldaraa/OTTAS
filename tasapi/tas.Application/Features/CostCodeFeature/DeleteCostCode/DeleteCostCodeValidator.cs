using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CostCodeFeature.UpdateCostCode;

namespace tas.Application.Features.CostCodeFeature.DeleteCostCode
{
    public sealed class DeleteCostCodeValidator : AbstractValidator<DeleteCostCodeRequest>
    {
        public DeleteCostCodeValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
