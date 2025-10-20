using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.StateFeature.CreateState
{
    public sealed class CreateStateValidator : AbstractValidator<CreateStateRequest>
    {
        public CreateStateValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
