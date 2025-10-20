using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.PositionFeature.DeletePosition
{
    public sealed class DeletePositionValidator : AbstractValidator<DeletePositionRequest>
    {
        public DeletePositionValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
