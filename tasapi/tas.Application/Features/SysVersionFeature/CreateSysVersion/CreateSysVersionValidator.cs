using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysVersionFeature.CreateSysVersion
{
    public sealed class CreateSysVersionValidator : AbstractValidator<CreateSysVersionRequest>
    {
        public CreateSysVersionValidator()
        {
            RuleFor(x => x.Version)
                .NotEmpty().WithMessage("Version number is required.")
                .MaximumLength(20).WithMessage("Version number cannot exceed 20 characters.")
                .Matches(@"^\d+(\.\d+){2}$").WithMessage("Invalid version number format. Must be in the format 'x.y.z'.");
            RuleFor(x => x.ReleaseNote).NotEmpty();

        }
    }
}
