using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ColorFeature.DeleteColor
{
    public sealed class DeleteColorValidator : AbstractValidator<DeleteColorRequest>
    {
        public DeleteColorValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
