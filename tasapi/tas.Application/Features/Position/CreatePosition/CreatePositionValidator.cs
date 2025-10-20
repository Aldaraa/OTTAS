using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.PositionFeature.CreatePosition
{
    public sealed class CreatePositionValidator : AbstractValidator<CreatePositionRequest>
    {
        public CreatePositionValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(150);
        }
    }
}
