using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CostCodeFeature.CreateCostCode;

namespace tas.Application.Features.CostCodeFeature.UpdateCostCode
{
    public sealed class UpdateCostCodeValidator : AbstractValidator<UpdateCostCodeRequest>
    {
        public UpdateCostCodeValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty().MinimumLength(3).MaximumLength(50);
            RuleFor(x => x.Number).NotEmpty().MaximumLength(30).MinimumLength(5);
        }
    }
}
