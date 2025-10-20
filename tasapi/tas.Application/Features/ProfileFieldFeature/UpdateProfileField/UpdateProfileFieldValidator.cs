using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ProfileFieldFeature.UpdateProfileField;

namespace tas.Application.Features.ProfileFieldFeature.UpdateProfileField
{
    public sealed class UpdateProfileFieldValidator : AbstractValidator<UpdateProfileFieldRequest>
    {
        public UpdateProfileFieldValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Label).NotEmpty().MaximumLength(50);
            RuleFor(x => x.FieldVisible).Must(x => x == 0 || x == 1).WithMessage("The value must be either 1 or 0."); 
            RuleFor(x => x.RequestVisible).Must(x => x == 0 || x == 1).WithMessage("The value must be either 1 or 0."); 
            RuleFor(x => x.FieldRequired).Must(x => x == 0 || x == 1).WithMessage("The value must be either 1 or 0."); 
            RuleFor(x => x.RequestRequired).Must(x => x == 0 || x == 1).WithMessage("The value must be either 1 or 0.");
            RuleFor(x => x.FieldReadOnly).Must(x => x == 0 || x == 1).WithMessage("The value must be either 1 or 0.");



        }
    }
}
