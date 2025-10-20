using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupFeature.UpdateRequestGroup
{
    public sealed class UpdateRequestGroupValidator : AbstractValidator<UpdateRequestGroupRequest>
    {
        public UpdateRequestGroupValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Description).NotEmpty().MaximumLength(100);
        }
    }
}
