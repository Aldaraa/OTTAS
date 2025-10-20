using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupFeature.CreateRequestGroup
{
    public sealed class CreateRequestGroupValidator : AbstractValidator<CreateRequestGroupRequest>
    {
        public CreateRequestGroupValidator()
        {
            RuleFor(x => x.Description).NotEmpty().MaximumLength(100);
        }
    }
}
