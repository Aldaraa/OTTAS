using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.PositionFeature.UpdatePosition;

namespace tas.Application.Features.PositioneFeature.UpdatePositione
{
    public sealed class UpdatePositionValidator : AbstractValidator<UpdatePositionRequest>
    {
        public UpdatePositionValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
