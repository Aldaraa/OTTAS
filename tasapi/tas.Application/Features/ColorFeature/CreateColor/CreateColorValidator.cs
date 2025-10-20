using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ColorFeature.CreateColor
{
    public sealed class CreateColorValidator : AbstractValidator<CreateColorRequest>
    {
        public CreateColorValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
