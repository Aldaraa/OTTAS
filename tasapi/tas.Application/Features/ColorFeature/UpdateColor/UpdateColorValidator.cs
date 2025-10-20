using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ColorFeature.UpdateColor;

namespace tas.Application.Features.ColoreFeature.UpdateColore
{
    public sealed class UpdateColorValidator : AbstractValidator<UpdateColorRequest>
    {
        public UpdateColorValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
