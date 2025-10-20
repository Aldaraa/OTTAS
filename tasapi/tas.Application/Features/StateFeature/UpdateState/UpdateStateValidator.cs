using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.UpdateState;

namespace tas.Application.Features.StateeFeature.UpdateStatee
{
    public sealed class UpdateStateValidator : AbstractValidator<UpdateStateRequest>
    {
        public UpdateStateValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
