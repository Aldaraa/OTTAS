using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.CostCodeFeature.CreateCostCode
{
    public sealed class CreateCostCodeValidator : AbstractValidator<CreateCostCodeRequest>
    {
        public CreateCostCodeValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty().MinimumLength(3).MaximumLength(100);
            RuleFor(x => x.Number).NotEmpty().MaximumLength(30).MinimumLength(5);
        }
    }
}
